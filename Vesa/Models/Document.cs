using Vesa.Models.Enums;

namespace Vesa.Models;

public class Document
{
    public Guid Id { get; set; }
    public Guid ApplicationId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public DocumentType DocumentType { get; set; }
    public DateTime UploadedAt { get; set; }

    // Navigation property
    public VisaApplication VisaApplication { get; set; } = null!;
}
