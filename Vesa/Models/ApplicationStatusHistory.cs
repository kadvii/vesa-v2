using Vesa.Models.Enums;

namespace Vesa.Models;

public class ApplicationStatusHistory
{
    public Guid Id { get; set; }
    public Guid ApplicationId { get; set; }
    public VisaApplicationStatus OldStatus { get; set; }
    public VisaApplicationStatus NewStatus { get; set; }
    public DateTime ChangedAt { get; set; }
    public string? ChangedByAdminId { get; set; }
    public string? Notes { get; set; }

    // Navigation properties
    public VisaApplication Application { get; set; } = null!;
    public AppUser? ChangedByAdmin { get; set; }
}
