using Vesa.Models.Enums;

namespace Vesa.Models;

public class VisaApplication
{
    public Guid Id { get; set; }
    public string ApplicantId { get; set; } = string.Empty;
    public Guid VisaTypeId { get; set; }
    public Guid CountryId { get; set; }
    public string PassportNumber { get; set; } = string.Empty;
    public DateOnly PassportExpiry { get; set; }
    public DateOnly TravelDateFrom { get; set; }
    public DateOnly TravelDateTo { get; set; }
    public VisaApplicationStatus Status { get; set; }
    public string? AdminNotes { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime SubmittedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewedByAdminId { get; set; }

    // Navigation properties
    public AppUser Applicant { get; set; } = null!;
    public VisaType VisaType { get; set; } = null!;
    public Country Country { get; set; } = null!;
    public AppUser? ReviewedByAdmin { get; set; }
    public ICollection<Document> Documents { get; set; } = new List<Document>();
}
