using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vesa.DTOs.Auth;
using Vesa.Services.Interfaces;

namespace Vesa.Controllers;

[ApiController]
[Route("api/app/profile")]
[Authorize(Roles = "Applicant")]
public class AppProfileController(IAuthService authService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var profile = await authService.GetProfileAsync(userId);
        if (profile is null)
            return NotFound();

        return Ok(profile);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var (success, error) = await authService.UpdateProfileAsync(userId, request);
        if (!success)
            return BadRequest(new { error });

        return Ok(new { message = "Profile updated successfully." });
    }

    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var (success, error) = await authService.ChangePasswordAsync(userId, request);
        if (!success)
            return BadRequest(new { error });

        return Ok(new { message = "Password changed successfully." });
    }
}
