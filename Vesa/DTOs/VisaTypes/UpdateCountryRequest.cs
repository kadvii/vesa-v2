namespace Vesa.DTOs.VisaTypes;

public class UpdateCountryRequest
{
    public string Name { get; set; } = string.Empty;
    public string IsoCode { get; set; } = string.Empty;
    public string FlagEmoji { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
