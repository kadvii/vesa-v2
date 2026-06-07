namespace Vesa.DTOs.VisaTypes;

public class UpdateVisaTypeRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ProcessingDays { get; set; }
    public decimal FeeAmount { get; set; }
    public List<string> RequiredDocuments { get; set; } = new();
    public bool IsActive { get; set; }
}
