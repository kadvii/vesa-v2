namespace Vesa.Models;

public class Country
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string IsoCode { get; set; } = string.Empty;
    public string FlagEmoji { get; set; } = string.Empty;
    public bool IsActive { get; set; }

    // Navigation properties
    public ICollection<VisaType> VisaTypes { get; set; } = new List<VisaType>();
    public ICollection<VisaApplication> VisaApplications { get; set; } = new List<VisaApplication>();
}
