namespace Vesa.DTOs.Documents;

public class DocumentResponse
{
    public Guid Id { get; set; }
    public Guid ApplicationId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
}
