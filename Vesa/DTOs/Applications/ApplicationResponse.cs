namespace Vesa.DTOs.Applications;

public class ApplicationResponse
{
    public Guid Id { get; set; }
    public string ApplicantId { get; set; } = string.Empty;
    public string ApplicantName { get; set; } = string.Empty;
    public Guid VisaTypeId { get; set; }
    public string VisaTypeName { get; set; } = string.Empty;
    public Guid CountryId { get; set; }
    public string CountryName { get; set; } = string.Empty;
    public string PassportNumber { get; set; } = string.Empty;
    public DateOnly PassportExpiry { get; set; }
    public DateOnly TravelDateFrom { get; set; }
    public DateOnly TravelDateTo { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? AdminNotes { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime SubmittedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewedByAdminId { get; set; }
}
