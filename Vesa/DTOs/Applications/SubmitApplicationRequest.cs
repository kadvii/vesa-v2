namespace Vesa.DTOs.Applications;

public class SubmitApplicationRequest
{
    public Guid VisaTypeId { get; set; }
    public Guid CountryId { get; set; }
    public string PassportNumber { get; set; } = string.Empty;
    public DateOnly PassportExpiry { get; set; }
    public DateOnly TravelDateFrom { get; set; }
    public DateOnly TravelDateTo { get; set; }
}
