using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vesa.DTOs.Appointments;

namespace Vesa.Services.Interfaces;

public interface IAppointmentSlotService
{
    Task<IList<AppointmentSlotResponse>> GetAvailableSlotsAsync(Guid countryId);
    Task<IList<AppointmentSlotResponse>> GetAllSlotsAsync();
    Task<(bool success, AppointmentSlotResponse? data, string? error)> CreateSlotAsync(CreateAppointmentSlotRequest request);
    Task<(bool success, AppointmentSlotResponse? data, string? error)> ToggleActiveAsync(Guid id);
    Task<(bool success, AppointmentSlotResponse? data, string? error)> UpdateCapacityAsync(Guid id, UpdateSlotCapacityRequest request);
}
