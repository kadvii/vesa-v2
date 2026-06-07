using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Vesa.Data;
using Vesa.DTOs.Payments;
using Vesa.Models;
using Vesa.Models.Enums;
using Vesa.Services.Interfaces;

namespace Vesa.Services;

public class PaymentService(AppDbContext db) : IPaymentService
{
    public async Task<PaymentResponse?> CreatePaymentAsync(Guid applicationId, string applicantId, decimal amount)
    {
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            ApplicationId = applicationId,
            ApplicantId = applicantId,
            Amount = amount,
            Currency = "IQD",
            Status = PaymentStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        db.Payments.Add(payment);
        await db.SaveChangesAsync();

        var applicant = await db.Users.FindAsync(applicantId);
        payment.Applicant = applicant!;

        return ToResponse(payment);
    }

    public async Task<(bool success, PaymentResponse? data, string? error)> ConfirmPaymentAsync(Guid id, ConfirmPaymentRequest request, string userId, bool isAdmin)
    {
        var payment = await db.Payments
            .Include(p => p.Applicant)
            .Include(p => p.VisaApplication)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (payment is null)
            return (false, null, "Payment not found.");

        if (!isAdmin && payment.ApplicantId != userId)
            return (false, null, "Unauthorized access to payment.");

        if (payment.Status == PaymentStatus.Paid)
            return (false, null, "Payment is already completed.");

        if (payment.Status == PaymentStatus.Refunded)
            return (false, null, "Cannot confirm a refunded payment.");

        payment.Status = PaymentStatus.Paid;
        payment.Method = request.Method;
        payment.TransactionReference = request.TransactionReference;
        payment.PaidAt = DateTime.UtcNow;

        await db.SaveChangesAsync();

        return (true, ToResponse(payment), null);
    }

    public async Task<PaymentResponse?> GetByApplicationIdAsync(Guid applicationId, string userId, bool isAdmin)
    {
        var payment = await db.Payments
            .Include(p => p.Applicant)
            .Include(p => p.VisaApplication)
            .FirstOrDefaultAsync(p => p.ApplicationId == applicationId);

        if (payment is null)
            return null;

        if (!isAdmin && payment.ApplicantId != userId)
            throw new UnauthorizedAccessException("You are not authorized to view this payment.");

        return ToResponse(payment);
    }

    public async Task<(bool success, PaymentResponse? data, string? error)> RefundAsync(Guid id)
    {
        var payment = await db.Payments
            .Include(p => p.Applicant)
            .Include(p => p.VisaApplication)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (payment is null)
            return (false, null, "Payment not found.");

        if (payment.Status != PaymentStatus.Paid)
            return (false, null, $"Only completed (Paid) payments can be refunded. Current status: '{payment.Status}'.");

        var appStatus = payment.VisaApplication.Status;
        if (appStatus != VisaApplicationStatus.Rejected && appStatus != VisaApplicationStatus.Cancelled)
            return (false, null, $"Refunds are only allowed for rejected or cancelled applications. Current application status: '{appStatus}'.");

        payment.Status = PaymentStatus.Refunded;
        await db.SaveChangesAsync();

        return (true, ToResponse(payment), null);
    }

    public async Task<IList<PaymentResponse>> GetAllAsync(PaymentStatus? status, DateTime? startDate, DateTime? endDate)
    {
        var query = db.Payments
            .Include(p => p.Applicant)
            .Include(p => p.VisaApplication)
            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(p => p.Status == status.Value);
        }

        if (startDate.HasValue)
        {
            query = query.Where(p => p.CreatedAt >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(p => p.CreatedAt <= endDate.Value);
        }

        var payments = await query
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return payments.Select(ToResponse).ToList();
    }

    private static PaymentResponse ToResponse(Payment p) => new()
    {
        Id = p.Id,
        ApplicationId = p.ApplicationId,
        ApplicantId = p.ApplicantId,
        ApplicantName = p.Applicant?.FullName ?? string.Empty,
        Amount = p.Amount,
        Currency = p.Currency,
        Status = p.Status.ToString(),
        Method = p.Method?.ToString(),
        TransactionReference = p.TransactionReference,
        PaidAt = p.PaidAt,
        CreatedAt = p.CreatedAt
    };
}
