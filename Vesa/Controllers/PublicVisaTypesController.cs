using Microsoft.AspNetCore.Mvc;
using Vesa.Services.Interfaces;

namespace Vesa.Controllers;

[ApiController]
[Route("api/public/visatypes")]
public class PublicVisaTypesController(IVisaTypeService visaTypeService) : ControllerBase
{
    [HttpGet("country/{countryId:guid}")]
    public async Task<IActionResult> GetByCountry(Guid countryId)
    {
        var types = await visaTypeService.GetVisaTypesByCountryAsync(countryId, activeOnly: true);
        return Ok(types);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var type = await visaTypeService.GetByIdAsync(id);
        if (type is null || !type.IsActive)
            return NotFound();

        return Ok(type);
    }
}
