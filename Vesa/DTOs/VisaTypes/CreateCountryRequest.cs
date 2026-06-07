namespace Vesa.DTOs.VisaTypes;

public class CreateCountryRequest
{
    public string Name { get; set; } = string.Empty;
    public string IsoCode { get; set; } = string.Empty;
    public string FlagEmoji { get; set; } = string.Empty;
}
