using Vesa.Models.Enums;

namespace Vesa.Models;

public class Notification
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public NotificationType Type { get; set; }

    // Navigation property
    public AppUser User { get; set; } = null!;
}
