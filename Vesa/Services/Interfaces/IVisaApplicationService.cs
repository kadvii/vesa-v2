using Vesa.DTOs.Applications;

namespace Vesa.Services.Interfaces;

public interface IVisaApplicationService
{
    Task<(bool success, ApplicationResponse? data, string? error)> SubmitAsync(SubmitApplicationRequest request, string applicantId);
    Task<IList<ApplicationResponse>> GetMyApplicationsAsync(string applicantId);
    Task<ApplicationResponse?> GetByIdAsync(Guid id, string userId, bool isAdmin);
    Task<(bool success, string? error)> CancelAsync(Guid id, string applicantId);
    Task<IList<ApplicationResponse>> GetAllAsync();
    Task<(bool success, string? error)> ChangeStatusAsync(Guid id, ChangeStatusRequest request, string adminId);
    Task<(bool success, string? error)> AddAdminNotesAsync(Guid id, AddAdminNotesRequest request);
    Task<(bool success, string? error)> RequestDocumentAsync(Guid id, RequestDocumentRequest request);
}
