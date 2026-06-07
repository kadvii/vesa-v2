namespace Vesa.Models;

public class VisaType
{
    public Guid Id { get; set; }
    public Guid CountryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ProcessingDays { get; set; }
    public decimal FeeAmount { get; set; }
    public bool IsActive { get; set; }
    public List<string> RequiredDocuments { get; set; } = new();

    // Navigation properties
    public Country Country { get; set; } = null!;
    public ICollection<VisaApplication> VisaApplications { get; set; } = new List<VisaApplication>();
}
