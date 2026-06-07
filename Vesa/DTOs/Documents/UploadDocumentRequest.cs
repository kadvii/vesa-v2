using Vesa.Models.Enums;

namespace Vesa.DTOs.Documents;

public class UploadDocumentRequest
{
    public Guid ApplicationId { get; set; }
    public DocumentType DocumentType { get; set; }
}
