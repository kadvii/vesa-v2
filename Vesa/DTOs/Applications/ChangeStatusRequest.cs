using Vesa.Models.Enums;

namespace Vesa.DTOs.Applications;

public class ChangeStatusRequest
{
    public VisaApplicationStatus Status { get; set; }
    public string? RejectionReason { get; set; }
    public string? AdminNotes { get; set; }
}
