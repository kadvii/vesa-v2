using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vesa.Services.Interfaces;

namespace Vesa.Controllers;

[ApiController]
[Route("api/admin/appointments")]
[Authorize(Roles = "Admin")]
public class AdminAppointmentsController(IAppointmentService appointmentService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var appointments = await appointmentService.GetAllAsync();
        return Ok(appointments);
    }

    [HttpPost("{id:guid}/confirm")]
    public async Task<IActionResult> Confirm(Guid id)
    {
        var (success, data, error) = await appointmentService.ConfirmAsync(id);
        if (!success)
            return BadRequest(new { error });

        return Ok(data);
    }

    [HttpPost("{id:guid}/no-show")]
    public async Task<IActionResult> MarkNoShow(Guid id)
    {
        var (success, data, error) = await appointmentService.MarkNoShowAsync(id);
        if (!success)
            return BadRequest(new { error });

        return Ok(data);
    }

    [HttpPost("{id:guid}/complete")]
    public async Task<IActionResult> Complete(Guid id)
    {
        var (success, data, error) = await appointmentService.CompleteAsync(id);
        if (!success)
            return BadRequest(new { error });

        return Ok(data);
    }
}
