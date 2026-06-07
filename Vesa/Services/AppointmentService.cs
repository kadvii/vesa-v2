using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Vesa.Data;
using Vesa.DTOs.Appointments;
using Vesa.Models;
using Vesa.Models.Enums;
using Vesa.Services.Interfaces;

namespace Vesa.Services;

public class AppointmentService(
    AppDbContext db,
    INotificationService notificationService) : IAppointmentService
{
    public async Task<(bool success, AppointmentResponse? data, string? error)> BookAsync(BookAppointmentRequest request, string applicantId)
    {
        var applicant = await db.Users.FindAsync(applicantId);
        if (applicant is null)
            return (false, null, "Applicant not found.");

        var app = await db.VisaApplications
            .Include(a => a.Country)
            .FirstOrDefaultAsync(a => a.Id == request.ApplicationId);

        if (app is null)
            return (false, null, "Visa application not found.");

        if (app.ApplicantId != applicantId)
            return (false, null, "Unauthorized application access.");

        if (app.Status != VisaApplicationStatus.Submitted && app.Status != VisaApplicationStatus.UnderReview)
            return (false, null, $"Cannot book appointment for application with status '{app.Status}'.");

        // Enforce: One active appointment per application
        var activeAppointment = await db.Appointments
            .AnyAsync(a => a.ApplicationId == request.ApplicationId && a.Status != AppointmentStatus.Cancelled);

        if (activeAppointment)
            return (false, null, "An active appointment already exists for this application.");

        var slot = await db.AppointmentSlots
            .Include(s => s.Country)
            .FirstOrDefaultAsync(s => s.Id == request.SlotId);

        if (slot is null || !slot.IsActive)
            return (false, null, "Selected slot does not exist or is inactive.");

        // Enforce: Slot country must match application country
        if (slot.CountryId != app.CountryId)
            return (false, null, $"The slot country ({slot.Country.Name}) does not match the application country ({app.Country.Name}).");

        // Enforce: Cannot book if slot is full
        if (slot.BookedCount >= slot.MaxCapacity)
            return (false, null, "Selected appointment slot is full.");

        var appointment = new Appointment
        {
            Id = Guid.NewGuid(),
            ApplicationId = request.ApplicationId,
            SlotId = request.SlotId,
            ApplicantId = applicantId,
            Status = AppointmentStatus.Booked,
            BookedAt = DateTime.UtcNow,
            IsReminderSent = false
        };

        slot.BookedCount++;
        db.Appointments.Add(appointment);
        await db.SaveChangesAsync();

        // Load navigations
        appointment.Applicant = applicant;
        appointment.AppointmentSlot = slot;

        // Send notification
        await notificationService.CreateAsync(
            applicantId,
            NotificationType.AppointmentBooked,
            "Appointment Booked",
            $"Your visa appointment for {app.Country.Name} has been booked on {slot.Date} at {slot.Time}."
        );

        return (true, ToResponse(appointment), null);
    }

    public async Task<(bool success, string? error)> CancelAsync(Guid id, string applicantId)
    {
        var appointment = await db.Appointments
            .Include(a => a.AppointmentSlot)
            .ThenInclude(s => s.Country)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (appointment is null)
            return (false, "Appointment not found.");

        if (appointment.ApplicantId != applicantId)
            return (false, "Unauthorized.");

        if (appointment.Status == AppointmentStatus.Cancelled)
            return (false, "Appointment is already cancelled.");

        if (appointment.Status != AppointmentStatus.Booked && appointment.Status != AppointmentStatus.Confirmed)
            return (false, $"Cannot cancel appointment with status '{appointment.Status}'.");

        // Enforce: Cancellation only allowed 24 hours before the appointment time
        var slotDateTime = appointment.AppointmentSlot.Date.ToDateTime(appointment.AppointmentSlot.Time, DateTimeKind.Utc);
        if (DateTime.UtcNow.AddHours(24) > slotDateTime)
            return (false, "Appointments can only be cancelled at least 24 hours in advance.");

        appointment.Status = AppointmentStatus.Cancelled;
        appointment.CancelledAt = DateTime.UtcNow;
        appointment.AppointmentSlot.BookedCount = Math.Max(0, appointment.AppointmentSlot.BookedCount - 1);

        await db.SaveChangesAsync();

        // Send notification
        await notificationService.CreateAsync(
            applicantId,
            NotificationType.StatusChanged,
            "Appointment Cancelled",
            $"Your appointment scheduled on {appointment.AppointmentSlot.Date} at {appointment.AppointmentSlot.Time} has been cancelled."
        );

        return (true, null);
    }

    public async Task<IList<AppointmentResponse>> GetMyAppointmentsAsync(string applicantId)
    {
        var appointments = await db.Appointments
            .Include(a => a.Applicant)
            .Include(a => a.AppointmentSlot)
            .ThenInclude(s => s.Country)
            .Where(a => a.ApplicantId == applicantId)
            .OrderByDescending(a => a.BookedAt)
            .ToListAsync();

        return appointments.Select(ToResponse).ToList();
    }

    public async Task<IList<AppointmentResponse>> GetAllAsync()
    {
        var appointments = await db.Appointments
            .Include(a => a.Applicant)
            .Include(a => a.AppointmentSlot)
            .ThenInclude(s => s.Country)
            .OrderByDescending(a => a.BookedAt)
            .ToListAsync();

        return appointments.Select(ToResponse).ToList();
    }

    public async Task<(bool success, AppointmentResponse? data, string? error)> ConfirmAsync(Guid id)
    {
        var appointment = await db.Appointments
            .Include(a => a.Applicant)
            .Include(a => a.AppointmentSlot)
            .ThenInclude(s => s.Country)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (appointment is null)
            return (false, null, "Appointment not found.");

        if (appointment.Status != AppointmentStatus.Booked)
            return (false, null, $"Only appointments in 'Booked' status can be confirmed. Current status: '{appointment.Status}'.");

        appointment.Status = AppointmentStatus.Confirmed;
        await db.SaveChangesAsync();

        // Send notification
        await notificationService.CreateAsync(
            appointment.ApplicantId,
            NotificationType.StatusChanged,
            "Appointment Confirmed",
            $"Your visa appointment scheduled on {appointment.AppointmentSlot.Date} at {appointment.AppointmentSlot.Time} has been confirmed."
        );

        return (true, ToResponse(appointment), null);
    }

    public async Task<(bool success, AppointmentResponse? data, string? error)> MarkNoShowAsync(Guid id)
    {
        var appointment = await db.Appointments
            .Include(a => a.Applicant)
            .Include(a => a.AppointmentSlot)
            .ThenInclude(s => s.Country)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (appointment is null)
            return (false, null, "Appointment not found.");

        if (appointment.Status != AppointmentStatus.Booked && appointment.Status != AppointmentStatus.Confirmed)
            return (false, null, $"Cannot mark appointment with status '{appointment.Status}' as No-Show.");

        appointment.Status = AppointmentStatus.NoShow;
        await db.SaveChangesAsync();

        return (true, ToResponse(appointment), null);
    }

    public async Task<(bool success, AppointmentResponse? data, string? error)> CompleteAsync(Guid id)
    {
        var appointment = await db.Appointments
            .Include(a => a.Applicant)
            .Include(a => a.AppointmentSlot)
            .ThenInclude(s => s.Country)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (appointment is null)
            return (false, null, "Appointment not found.");

        if (appointment.Status != AppointmentStatus.Booked && appointment.Status != AppointmentStatus.Confirmed)
            return (false, null, $"Cannot mark appointment with status '{appointment.Status}' as Completed.");

        appointment.Status = AppointmentStatus.Completed;
        await db.SaveChangesAsync();

        return (true, ToResponse(appointment), null);
    }

    private static AppointmentResponse ToResponse(Appointment a) => new()
    {
        Id = a.Id,
        ApplicationId = a.ApplicationId,
        SlotId = a.SlotId,
        ApplicantId = a.ApplicantId,
        ApplicantName = a.Applicant?.FullName ?? string.Empty,
        Status = a.Status.ToString(),
        BookedAt = a.BookedAt,
        CancelledAt = a.CancelledAt,
        Notes = a.Notes,
        IsReminderSent = a.IsReminderSent,
        SlotDate = a.AppointmentSlot?.Date ?? default,
        SlotTime = a.AppointmentSlot?.Time ?? default,
        CountryName = a.AppointmentSlot?.Country?.Name ?? string.Empty
    };
}
