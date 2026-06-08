namespace Vesa.DTOs.Applications;

public class ApplicationStatusHistoryResponse
{
    public Guid Id { get; set; }
    public Guid ApplicationId { get; set; }
    public string OldStatus { get; set; } = string.Empty;
    public string NewStatus { get; set; } = string.Empty;
    public DateTime ChangedAt { get; set; }
    public string? ChangedByAdminId { get; set; }
    public string? Notes { get; set; }
}
