using Microsoft.EntityFrameworkCore;
using Vesa.Data;
using Vesa.DTOs.Notifications;
using Vesa.Models;
using Vesa.Models.Enums;
using Vesa.Services.Interfaces;

namespace Vesa.Services;

public class NotificationService(AppDbContext db) : INotificationService
{
    public async Task<NotificationResponse> CreateAsync(string userId, NotificationType type, string title, string message)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Type = type,
            Title = title,
            Message = message,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        db.Notifications.Add(notification);
        await db.SaveChangesAsync();

        return ToResponse(notification);
    }

    public async Task<IList<NotificationResponse>> GetMyNotificationsAsync(string userId)
    {
        var notifications = await db.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

        return notifications.Select(ToResponse).ToList();
    }

    public async Task<(bool success, string? error)> MarkAsReadAsync(Guid id, string userId)
    {
        var notification = await db.Notifications.FindAsync(id);
        if (notification is null)
            return (false, "Notification not found.");

        if (notification.UserId != userId)
            return (false, "Unauthorized.");

        notification.IsRead = true;
        await db.SaveChangesAsync();

        return (true, null);
    }

    private static NotificationResponse ToResponse(Notification n) => new()
    {
        Id = n.Id,
        Title = n.Title,
        Message = n.Message,
        IsRead = n.IsRead,
        CreatedAt = n.CreatedAt,
        Type = n.Type.ToString()
    };
}
