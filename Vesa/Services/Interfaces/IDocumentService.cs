using Microsoft.AspNetCore.Http;
using Vesa.DTOs.Documents;
using Vesa.Models.Enums;

namespace Vesa.Services.Interfaces;

public interface IDocumentService
{
    Task<(bool success, DocumentResponse? data, string? error)> UploadAsync(IFormFile file, Guid applicationId, DocumentType documentType, string requestingUserId, bool isAdmin);
    Task<IList<DocumentResponse>> GetByApplicationIdAsync(Guid applicationId, string userId, bool isAdmin);
}
