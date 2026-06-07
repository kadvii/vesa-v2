using Microsoft.AspNetCore.Mvc;
using Vesa.DTOs.Auth;
using Vesa.Services.Interfaces;

namespace Vesa.Controllers;

[ApiController]
[Route("api/admin/auth")]
public class AdminAuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var (success, data, error) = await authService.LoginAsync(request);
        if (!success)
            return BadRequest(new { error });

        // Verify if user is actually Admin
        if (data is null || !data.Roles.Contains("Admin"))
            return Forbid();

        return Ok(data);
    }
}
