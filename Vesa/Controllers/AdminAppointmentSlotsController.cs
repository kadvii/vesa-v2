using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vesa.DTOs.Appointments;
using Vesa.Services.Interfaces;

namespace Vesa.Controllers;

[ApiController]
[Route("api/admin/appointment-slots")]
[Authorize(Roles = "Admin")]
public class AdminAppointmentSlotsController(IAppointmentSlotService slotService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAppointmentSlotRequest request)
    {
        var (success, data, error) = await slotService.CreateSlotAsync(request);
        if (!success)
            return BadRequest(new { error });

        return Ok(data);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var slots = await slotService.GetAllSlotsAsync();
        return Ok(slots);
    }

    [HttpPost("{id:guid}/toggle")]
    public async Task<IActionResult> ToggleActive(Guid id)
    {
        var (success, data, error) = await slotService.ToggleActiveAsync(id);
        if (!success)
            return BadRequest(new { error });

        return Ok(data);
    }

    [HttpPut("{id:guid}/capacity")]
    public async Task<IActionResult> UpdateCapacity(Guid id, [FromBody] UpdateSlotCapacityRequest request)
    {
        var (success, data, error) = await slotService.UpdateCapacityAsync(id, request);
        if (!success)
            return BadRequest(new { error });

        return Ok(data);
    }
}
