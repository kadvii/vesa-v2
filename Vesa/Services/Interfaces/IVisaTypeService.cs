using Vesa.DTOs.VisaTypes;

namespace Vesa.Services.Interfaces;

public interface IVisaTypeService
{
    Task<IList<VisaTypeResponse>> GetVisaTypesByCountryAsync(Guid countryId, bool activeOnly = true);
    Task<VisaTypeResponse?> GetByIdAsync(Guid id);
    Task<(bool success, VisaTypeResponse? data, string? error)> CreateAsync(CreateVisaTypeRequest request);
    Task<(bool success, VisaTypeResponse? data, string? error)> UpdateAsync(Guid id, UpdateVisaTypeRequest request);
    Task<(bool success, string? error)> ToggleActiveAsync(Guid id);
}
