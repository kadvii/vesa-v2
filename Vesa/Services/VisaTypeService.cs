using Microsoft.EntityFrameworkCore;
using Vesa.Data;
using Vesa.DTOs.VisaTypes;
using Vesa.Models;
using Vesa.Services.Interfaces;

namespace Vesa.Services;

public class VisaTypeService(AppDbContext db) : IVisaTypeService
{
    public async Task<IList<VisaTypeResponse>> GetVisaTypesByCountryAsync(Guid countryId, bool activeOnly = true)
    {
        var query = db.VisaTypes
            .Include(v => v.Country)
            .Where(v => v.CountryId == countryId);

        if (activeOnly)
        {
            query = query.Where(v => v.IsActive);
        }

        var types = await query.OrderBy(v => v.Name).ToListAsync();
        return types.Select(ToResponse).ToList();
    }

    public async Task<VisaTypeResponse?> GetByIdAsync(Guid id)
    {
        var type = await db.VisaTypes
            .Include(v => v.Country)
            .FirstOrDefaultAsync(v => v.Id == id);

        return type is null ? null : ToResponse(type);
    }

    public async Task<(bool success, VisaTypeResponse? data, string? error)> CreateAsync(CreateVisaTypeRequest request)
    {
        var country = await db.Countries.FindAsync(request.CountryId);
        if (country is null)
            return (false, null, "Selected country does not exist.");

        var nameExists = await db.VisaTypes.AnyAsync(v => v.CountryId == request.CountryId && v.Name.ToLower() == request.Name.ToLower());
        if (nameExists)
            return (false, null, "Visa type name already exists for this country.");

        var visaType = new VisaType
        {
            Id = Guid.NewGuid(),
            CountryId = request.CountryId,
            Name = request.Name,
            Description = request.Description,
            ProcessingDays = request.ProcessingDays,
            FeeAmount = request.FeeAmount,
            RequiredDocuments = request.RequiredDocuments,
            IsActive = true
        };

        db.VisaTypes.Add(visaType);
        await db.SaveChangesAsync();

        var detail = await GetByIdAsync(visaType.Id);
        return (true, detail, null);
    }

    public async Task<(bool success, VisaTypeResponse? data, string? error)> UpdateAsync(Guid id, UpdateVisaTypeRequest request)
    {
        var visaType = await db.VisaTypes
            .Include(v => v.Country)
            .FirstOrDefaultAsync(v => v.Id == id);

        if (visaType is null)
            return (false, null, "Visa type not found.");

        var nameExists = await db.VisaTypes.AnyAsync(v => v.CountryId == visaType.CountryId && v.Name.ToLower() == request.Name.ToLower() && v.Id != id);
        if (nameExists)
            return (false, null, "Visa type name already exists for this country.");

        visaType.Name = request.Name;
        visaType.Description = request.Description;
        visaType.ProcessingDays = request.ProcessingDays;
        visaType.FeeAmount = request.FeeAmount;
        visaType.RequiredDocuments = request.RequiredDocuments;
        visaType.IsActive = request.IsActive;

        await db.SaveChangesAsync();

        var detail = await GetByIdAsync(visaType.Id);
        return (true, detail, null);
    }

    public async Task<(bool success, string? error)> ToggleActiveAsync(Guid id)
    {
        var type = await db.VisaTypes.FindAsync(id);
        if (type is null)
            return (false, "Visa type not found.");

        type.IsActive = !type.IsActive;
        await db.SaveChangesAsync();
        return (true, null);
    }

    private static VisaTypeResponse ToResponse(VisaType v) => new()
    {
        Id = v.Id,
        CountryId = v.CountryId,
        CountryName = v.Country?.Name ?? string.Empty,
        Name = v.Name,
        Description = v.Description,
        ProcessingDays = v.ProcessingDays,
        FeeAmount = v.FeeAmount,
        IsActive = v.IsActive,
        RequiredDocuments = v.RequiredDocuments
    };
}
