using Microsoft.AspNetCore.Mvc;
using Vesa.DTOs.Auth;
using Vesa.Services.Interfaces;

namespace Vesa.Controllers;

[ApiController]
[Route("api/app/auth")]
public class AppAuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var (success, data, error) = await authService.RegisterAsync(request);
        if (!success)
            return BadRequest(new { error });

        return Ok(data);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var (success, data, error) = await authService.LoginAsync(request);
        if (!success)
            return BadRequest(new { error });

        return Ok(data);
    }
}
