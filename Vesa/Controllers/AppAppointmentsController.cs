using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vesa.DTOs.Appointments;
using Vesa.Services.Interfaces;

namespace Vesa.Controllers;

[ApiController]
[Route("api/app/appointments")]
[Authorize(Roles = "Applicant")]
public class AppAppointmentsController(
    IAppointmentService appointmentService,
    IAppointmentSlotService slotService) : ControllerBase
{
    [HttpPost("book")]
    public async Task<IActionResult> Book([FromBody] BookAppointmentRequest request)
    {
        var applicantId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(applicantId))
            return Unauthorized();

        var (success, data, error) = await appointmentService.BookAsync(request, applicantId);
        if (!success)
            return BadRequest(new { error });

        return Ok(data);
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var applicantId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(applicantId))
            return Unauthorized();

        var (success, error) = await appointmentService.CancelAsync(id, applicantId);
        if (!success)
            return BadRequest(new { error });

        return Ok(new { message = "Appointment cancelled successfully." });
    }

    [HttpGet]
    public async Task<IActionResult> GetMyAppointments()
    {
        var applicantId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(applicantId))
            return Unauthorized();

        var appointments = await appointmentService.GetMyAppointmentsAsync(applicantId);
        return Ok(appointments);
    }

    [HttpGet("slots")]
    public async Task<IActionResult> GetAvailableSlots([FromQuery] Guid countryId)
    {
        var slots = await slotService.GetAvailableSlotsAsync(countryId);
        return Ok(slots);
    }
}
