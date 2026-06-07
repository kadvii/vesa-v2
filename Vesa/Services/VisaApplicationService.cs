using Microsoft.EntityFrameworkCore;
using Vesa.Data;
using Vesa.DTOs.Applications;
using Vesa.Models;
using Vesa.Models.Enums;
using Vesa.Services.Interfaces;

namespace Vesa.Services;

public class VisaApplicationService(
    AppDbContext db,
    INotificationService notificationService,
    IPaymentService paymentService) : IVisaApplicationService
{
    public async Task<(bool success, ApplicationResponse? data, string? error)> SubmitAsync(SubmitApplicationRequest request, string applicantId)
    {
        var applicant = await db.Users.FindAsync(applicantId);
        if (applicant is null)
            return (false, null, "Applicant not found.");

        var country = await db.Countries.FindAsync(request.CountryId);
        if (country is null || !country.IsActive)
            return (false, null, "Selected country is not active or does not exist.");

        var visaType = await db.VisaTypes.FindAsync(request.VisaTypeId);
        if (visaType is null || !visaType.IsActive || visaType.CountryId != request.CountryId)
            return (false, null, "Selected visa type is not active or does not belong to the selected country.");

        var application = new VisaApplication
        {
            Id = Guid.NewGuid(),
            ApplicantId = applicantId,
            VisaTypeId = request.VisaTypeId,
            CountryId = request.CountryId,
            PassportNumber = request.PassportNumber,
            PassportExpiry = request.PassportExpiry,
            TravelDateFrom = request.TravelDateFrom,
            TravelDateTo = request.TravelDateTo,
            Status = VisaApplicationStatus.Submitted,
            SubmittedAt = DateTime.UtcNow
        };

        db.VisaApplications.Add(application);
        await db.SaveChangesAsync();

        await paymentService.CreatePaymentAsync(application.Id, applicantId, visaType.FeeAmount);

        await notificationService.CreateAsync(
            applicantId,
            NotificationType.ApplicationSubmitted,
            "Application Submitted",
            $"Your visa application for {country.Name} ({visaType.Name}) has been submitted successfully."
        );

        var details = await GetByIdAsync(application.Id, applicantId, isAdmin: false);
        return (true, details, null);
    }

    public async Task<IList<ApplicationResponse>> GetMyApplicationsAsync(string applicantId)
    {
        var apps = await db.VisaApplications
            .Include(a => a.Applicant)
            .Include(a => a.VisaType)
            .Include(a => a.Country)
            .Where(a => a.ApplicantId == applicantId)
            .OrderByDescending(a => a.SubmittedAt)
            .ToListAsync();

        return apps.Select(ToResponse).ToList();
    }

    public async Task<ApplicationResponse?> GetByIdAsync(Guid id, string userId, bool isAdmin)
    {
        var app = await db.VisaApplications
            .Include(a => a.Applicant)
            .Include(a => a.VisaType)
            .Include(a => a.Country)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (app is null)
            return null;

        if (!isAdmin && app.ApplicantId != userId)
            throw new UnauthorizedAccessException("You are not authorized to view this application.");

        return ToResponse(app);
    }

    public async Task<(bool success, string? error)> CancelAsync(Guid id, string applicantId)
    {
        var app = await db.VisaApplications.FindAsync(id);
        if (app is null)
            return (false, "Application not found.");

        if (app.ApplicantId != applicantId)
            return (false, "Unauthorized.");

        if (app.Status != VisaApplicationStatus.Submitted && app.Status != VisaApplicationStatus.UnderReview)
            return (false, "Cannot cancel application at this stage.");

        app.Status = VisaApplicationStatus.Cancelled;
        app.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();

        await notificationService.CreateAsync(
            applicantId,
            NotificationType.StatusChanged,
            "Application Cancelled",
            "You have cancelled your visa application."
        );

        return (true, null);
    }

    public async Task<IList<ApplicationResponse>> GetAllAsync()
    {
        var apps = await db.VisaApplications
            .Include(a => a.Applicant)
            .Include(a => a.VisaType)
            .Include(a => a.Country)
            .OrderByDescending(a => a.SubmittedAt)
            .ToListAsync();

        return apps.Select(ToResponse).ToList();
    }

    public async Task<(bool success, string? error)> ChangeStatusAsync(Guid id, ChangeStatusRequest request, string adminId)
    {
        var app = await db.VisaApplications
            .Include(a => a.Country)
            .Include(a => a.VisaType)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (app is null)
            return (false, "Application not found.");

        if (request.Status == VisaApplicationStatus.Approved)
        {
            var payment = await db.Payments.FirstOrDefaultAsync(p => p.ApplicationId == id);
            if (payment is null || payment.Status != PaymentStatus.Paid)
            {
                return (false, "Cannot approve application because payment is not completed.");
            }
        }

        app.Status = request.Status;
        app.ReviewedByAdminId = adminId;
        app.ReviewedAt = DateTime.UtcNow;
        app.UpdatedAt = DateTime.UtcNow;

        if (request.Status == VisaApplicationStatus.Rejected)
        {
            app.RejectionReason = request.RejectionReason;
        }
        else
        {
            app.RejectionReason = null;
        }

        await db.SaveChangesAsync();

        await notificationService.CreateAsync(
            app.ApplicantId,
            NotificationType.StatusChanged,
            "Application Status Updated",
            $"Your visa application for {app.Country.Name} status has been updated to {request.Status}."
        );

        return (true, null);
    }

    public async Task<(bool success, string? error)> AddAdminNotesAsync(Guid id, AddAdminNotesRequest request)
    {
        var app = await db.VisaApplications.FindAsync(id);
        if (app is null)
            return (false, "Application not found.");

        app.AdminNotes = request.AdminNotes;
        app.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return (true, null);
    }

    public async Task<(bool success, string? error)> RequestDocumentAsync(Guid id, RequestDocumentRequest request)
    {
        var app = await db.VisaApplications.FindAsync(id);
        if (app is null)
            return (false, "Application not found.");

        // Change status to UnderReview if it was Submitted
        if (app.Status == VisaApplicationStatus.Submitted)
        {
            app.Status = VisaApplicationStatus.UnderReview;
            app.UpdatedAt = DateTime.UtcNow;
        }

        await db.SaveChangesAsync();

        await notificationService.CreateAsync(
            app.ApplicantId,
            NotificationType.DocumentRequested,
            "Additional Document Requested",
            request.Message
        );

        return (true, null);
    }

    private static ApplicationResponse ToResponse(VisaApplication a) => new()
    {
        Id = a.Id,
        ApplicantId = a.ApplicantId,
        ApplicantName = a.Applicant?.FullName ?? string.Empty,
        VisaTypeId = a.VisaTypeId,
        VisaTypeName = a.VisaType?.Name ?? string.Empty,
        CountryId = a.CountryId,
        CountryName = a.Country?.Name ?? string.Empty,
        PassportNumber = a.PassportNumber,
        PassportExpiry = a.PassportExpiry,
        TravelDateFrom = a.TravelDateFrom,
        TravelDateTo = a.TravelDateTo,
        Status = a.Status.ToString(),
        AdminNotes = a.AdminNotes,
        RejectionReason = a.RejectionReason,
        SubmittedAt = a.SubmittedAt,
        ReviewedAt = a.ReviewedAt,
        ReviewedByAdminId = a.ReviewedByAdminId
    };
}
