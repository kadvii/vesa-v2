using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Vesa.Data;
using Vesa.Models.Enums;
using Vesa.Services.Interfaces;

namespace Vesa.Services.Background;

public class AppointmentReminderBackgroundService(
    IServiceScopeFactory scopeFactory,
    ILogger<AppointmentReminderBackgroundService> logger) : BackgroundService
{
    private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(15);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Appointment Reminder Background Service is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await SendRemindersAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while sending appointment reminders.");
            }

            try
            {
                await Task.Delay(_checkInterval, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }

        logger.LogInformation("Appointment Reminder Background Service is stopping.");
    }

    private async Task SendRemindersAsync(CancellationToken stoppingToken)
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

        var tomorrow = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

        // Get appointments with pending reminders up to tomorrow's date to minimize in-memory list
        var pendingAppointments = await db.Appointments
            .Include(a => a.AppointmentSlot)
            .ThenInclude(s => s.Country)
            .Where(a => !a.IsReminderSent &&
                        (a.Status == AppointmentStatus.Booked || a.Status == AppointmentStatus.Confirmed) &&
                        a.AppointmentSlot.Date <= tomorrow)
            .ToListAsync(stoppingToken);

        var now = DateTime.UtcNow;
        var reminderThreshold = now.AddHours(24);

        foreach (var app in pendingAppointments)
        {
            var slotDateTime = app.AppointmentSlot.Date.ToDateTime(app.AppointmentSlot.Time, DateTimeKind.Utc);

            // Send reminder if appointment is in the next 24 hours and is still in the future
            if (slotDateTime <= reminderThreshold && slotDateTime > now)
            {
                try
                {
                    await notificationService.CreateAsync(
                        app.ApplicantId,
                        NotificationType.AppointmentReminder,
                        "Appointment Reminder",
                        $"Reminder: You have an upcoming visa appointment for {app.AppointmentSlot.Country.Name} on {app.AppointmentSlot.Date} at {app.AppointmentSlot.Time}."
                    );

                    app.IsReminderSent = true;
                    logger.LogInformation("Sent appointment reminder for appointment {AppointmentId}", app.Id);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to send reminder for appointment {AppointmentId}", app.Id);
                }
            }
        }

        if (pendingAppointments.Any(a => a.IsReminderSent))
        {
            await db.SaveChangesAsync(stoppingToken);
        }
    }
}
