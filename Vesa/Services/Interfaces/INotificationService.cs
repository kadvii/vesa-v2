using Vesa.DTOs.Notifications;
using Vesa.Models.Enums;

namespace Vesa.Services.Interfaces;

public interface INotificationService
{
    Task<NotificationResponse> CreateAsync(string userId, NotificationType type, string title, string message);
    Task<IList<NotificationResponse>> GetMyNotificationsAsync(string userId);
    Task<(bool success, string? error)> MarkAsReadAsync(Guid id, string userId);
}
