namespace Vesa.DTOs.VisaTypes;

public class VisaTypeResponse
{
    public Guid Id { get; set; }
    public Guid CountryId { get; set; }
    public string CountryName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ProcessingDays { get; set; }
    public decimal FeeAmount { get; set; }
    public bool IsActive { get; set; }
    public List<string> RequiredDocuments { get; set; } = new();
}
