using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Vesa.Data;
using Vesa.DTOs.Appointments;
using Vesa.Models;
using Vesa.Services.Interfaces;

namespace Vesa.Services;

public class AppointmentSlotService(AppDbContext db) : IAppointmentSlotService
{
    public async Task<IList<AppointmentSlotResponse>> GetAvailableSlotsAsync(Guid countryId)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var slots = await db.AppointmentSlots
            .Include(s => s.Country)
            .Where(s => s.CountryId == countryId && s.IsActive && s.Date >= today && s.BookedCount < s.MaxCapacity)
            .OrderBy(s => s.Date)
            .ThenBy(s => s.Time)
            .ToListAsync();

        return slots.Select(ToResponse).ToList();
    }

    public async Task<IList<AppointmentSlotResponse>> GetAllSlotsAsync()
    {
        var slots = await db.AppointmentSlots
            .Include(s => s.Country)
            .OrderBy(s => s.Date)
            .ThenBy(s => s.Time)
            .ToListAsync();

        return slots.Select(ToResponse).ToList();
    }

    public async Task<(bool success, AppointmentSlotResponse? data, string? error)> CreateSlotAsync(CreateAppointmentSlotRequest request)
    {
        var country = await db.Countries.FindAsync(request.CountryId);
        if (country is null)
            return (false, null, "Country not found.");

        var slot = new AppointmentSlot
        {
            Id = Guid.NewGuid(),
            CountryId = request.CountryId,
            Date = request.Date,
            Time = request.Time,
            MaxCapacity = request.MaxCapacity,
            BookedCount = 0,
            IsActive = true
        };

        db.AppointmentSlots.Add(slot);
        await db.SaveChangesAsync();

        // Load country navigation property
        slot.Country = country;

        return (true, ToResponse(slot), null);
    }

    public async Task<(bool success, AppointmentSlotResponse? data, string? error)> ToggleActiveAsync(Guid id)
    {
        var slot = await db.AppointmentSlots
            .Include(s => s.Country)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (slot is null)
            return (false, null, "Appointment slot not found.");

        slot.IsActive = !slot.IsActive;
        await db.SaveChangesAsync();

        return (true, ToResponse(slot), null);
    }

    public async Task<(bool success, AppointmentSlotResponse? data, string? error)> UpdateCapacityAsync(Guid id, UpdateSlotCapacityRequest request)
    {
        var slot = await db.AppointmentSlots
            .Include(s => s.Country)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (slot is null)
            return (false, null, "Appointment slot not found.");

        if (request.MaxCapacity < slot.BookedCount)
            return (false, null, $"New capacity cannot be less than current booked count ({slot.BookedCount}).");

        slot.MaxCapacity = request.MaxCapacity;
        await db.SaveChangesAsync();

        return (true, ToResponse(slot), null);
    }

    private static AppointmentSlotResponse ToResponse(AppointmentSlot s) => new()
    {
        Id = s.Id,
        Date = s.Date,
        Time = s.Time,
        CountryId = s.CountryId,
        CountryName = s.Country?.Name ?? string.Empty,
        MaxCapacity = s.MaxCapacity,
        BookedCount = s.BookedCount,
        IsActive = s.IsActive
    };
}
