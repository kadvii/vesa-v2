using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vesa.DTOs.Appointments;

namespace Vesa.Services.Interfaces;

public interface IAppointmentService
{
    Task<(bool success, AppointmentResponse? data, string? error)> BookAsync(BookAppointmentRequest request, string applicantId);
    Task<(bool success, string? error)> CancelAsync(Guid id, string applicantId);
    Task<IList<AppointmentResponse>> GetMyAppointmentsAsync(string applicantId);
    Task<IList<AppointmentResponse>> GetAllAsync();
    Task<(bool success, AppointmentResponse? data, string? error)> ConfirmAsync(Guid id);
    Task<(bool success, AppointmentResponse? data, string? error)> MarkNoShowAsync(Guid id);
    Task<(bool success, AppointmentResponse? data, string? error)> CompleteAsync(Guid id);
}
