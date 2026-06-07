using Vesa.DTOs.VisaTypes;

namespace Vesa.Services.Interfaces;

public interface ICountryService
{
    Task<IList<CountryResponse>> GetActiveCountriesAsync();
    Task<IList<CountryResponse>> GetAllCountriesAsync();
    Task<(bool success, CountryResponse? data, string? error)> CreateAsync(CreateCountryRequest request);
    Task<(bool success, CountryResponse? data, string? error)> UpdateAsync(Guid id, UpdateCountryRequest request);
    Task<(bool success, string? error)> ToggleActiveAsync(Guid id);
}
