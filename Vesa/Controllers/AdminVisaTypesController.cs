using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vesa.DTOs.VisaTypes;
using Vesa.Services.Interfaces;

namespace Vesa.Controllers;

[ApiController]
[Route("api/admin/visatypes")]
[Authorize(Roles = "Admin")]
public class AdminVisaTypesController(IVisaTypeService visaTypeService) : ControllerBase
{
    [HttpGet("country/{countryId:guid}")]
    public async Task<IActionResult> GetByCountry(Guid countryId)
    {
        var types = await visaTypeService.GetVisaTypesByCountryAsync(countryId, activeOnly: false);
        return Ok(types);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateVisaTypeRequest request)
    {
        var (success, data, error) = await visaTypeService.CreateAsync(request);
        if (!success)
            return BadRequest(new { error });

        return Ok(data);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateVisaTypeRequest request)
    {
        var (success, data, error) = await visaTypeService.UpdateAsync(id, request);
        if (!success)
            return BadRequest(new { error });

        return Ok(data);
    }

    [HttpPost("{id:guid}/toggle")]
    public async Task<IActionResult> Toggle(Guid id)
    {
        var (success, error) = await visaTypeService.ToggleActiveAsync(id);
        if (!success)
            return BadRequest(new { error });

        return Ok(new { message = "Visa type active status toggled successfully." });
    }
}
