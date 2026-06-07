using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vesa.DTOs.VisaTypes;
using Vesa.Services.Interfaces;

namespace Vesa.Controllers;

[ApiController]
[Route("api/admin/countries")]
[Authorize(Roles = "Admin")]
public class AdminCountriesController(ICountryService countryService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var countries = await countryService.GetAllCountriesAsync();
        return Ok(countries);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCountryRequest request)
    {
        var (success, data, error) = await countryService.CreateAsync(request);
        if (!success)
            return BadRequest(new { error });

        return Ok(data);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCountryRequest request)
    {
        var (success, data, error) = await countryService.UpdateAsync(id, request);
        if (!success)
            return BadRequest(new { error });

        return Ok(data);
    }

    [HttpPost("{id:guid}/toggle")]
    public async Task<IActionResult> Toggle(Guid id)
    {
        var (success, error) = await countryService.ToggleActiveAsync(id);
        if (!success)
            return BadRequest(new { error });

        return Ok(new { message = "Country active status toggled successfully." });
    }
}
