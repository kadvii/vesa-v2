using Microsoft.AspNetCore.Mvc;
using Vesa.Services.Interfaces;

namespace Vesa.Controllers;

[ApiController]
[Route("api/public/countries")]
public class PublicCountriesController(ICountryService countryService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetActiveCountries()
    {
        var countries = await countryService.GetActiveCountriesAsync();
        return Ok(countries);
    }
}
