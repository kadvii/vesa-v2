using Microsoft.EntityFrameworkCore;
using Vesa.Data;
using Vesa.DTOs.VisaTypes;
using Vesa.Models;
using Vesa.Services.Interfaces;

namespace Vesa.Services;

public class CountryService(AppDbContext db) : ICountryService
{
    public async Task<IList<CountryResponse>> GetActiveCountriesAsync()
    {
        var countries = await db.Countries
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();

        return countries.Select(ToResponse).ToList();
    }

    public async Task<IList<CountryResponse>> GetAllCountriesAsync()
    {
        var countries = await db.Countries
            .OrderBy(c => c.Name)
            .ToListAsync();

        return countries.Select(ToResponse).ToList();
    }

    public async Task<(bool success, CountryResponse? data, string? error)> CreateAsync(CreateCountryRequest request)
    {
        var nameExists = await db.Countries.AnyAsync(c => c.Name.ToLower() == request.Name.ToLower());
        if (nameExists)
            return (false, null, "Country name already exists.");

        var codeExists = await db.Countries.AnyAsync(c => c.IsoCode.ToLower() == request.IsoCode.ToLower());
        if (codeExists)
            return (false, null, "Country ISO code already exists.");

        var country = new Country
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            IsoCode = request.IsoCode,
            FlagEmoji = request.FlagEmoji,
            IsActive = true
        };

        db.Countries.Add(country);
        await db.SaveChangesAsync();

        return (true, ToResponse(country), null);
    }

    public async Task<(bool success, CountryResponse? data, string? error)> UpdateAsync(Guid id, UpdateCountryRequest request)
    {
        var country = await db.Countries.FindAsync(id);
        if (country is null)
            return (false, null, "Country not found.");

        var nameExists = await db.Countries.AnyAsync(c => c.Name.ToLower() == request.Name.ToLower() && c.Id != id);
        if (nameExists)
            return (false, null, "Country name already exists.");

        var codeExists = await db.Countries.AnyAsync(c => c.IsoCode.ToLower() == request.IsoCode.ToLower() && c.Id != id);
        if (codeExists)
            return (false, null, "Country ISO code already exists.");

        country.Name = request.Name;
        country.IsoCode = request.IsoCode;
        country.FlagEmoji = request.FlagEmoji;
        country.IsActive = request.IsActive;

        await db.SaveChangesAsync();
        return (true, ToResponse(country), null);
    }

    public async Task<(bool success, string? error)> ToggleActiveAsync(Guid id)
    {
        var country = await db.Countries.FindAsync(id);
        if (country is null)
            return (false, "Country not found.");

        country.IsActive = !country.IsActive;
        await db.SaveChangesAsync();
        return (true, null);
    }

    private static CountryResponse ToResponse(Country c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        IsoCode = c.IsoCode,
        FlagEmoji = c.FlagEmoji,
        IsActive = c.IsActive
    };
}
