using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vesa.DTOs.Users;
using Vesa.Services.Interfaces;

namespace Vesa.Controllers;

[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = "Admin")]
public class AdminUsersController(IUserService userService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search)
    {
        var users = await userService.GetAllAsync(search);
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var user = await userService.GetByIdAsync(id);
        if (user is null)
            return NotFound();

        return Ok(user);
    }

    [HttpPut("{id}/restrict")]
    public async Task<IActionResult> Restrict(string id, [FromBody] RestrictUserRequest request)
    {
        var (success, error) = await userService.RestrictAsync(id, request.Reason);
        if (!success)
            return BadRequest(new { error });

        return Ok(new { message = "User restricted successfully." });
    }

    [HttpPut("{id}/unrestrict")]
    public async Task<IActionResult> Unrestrict(string id)
    {
        var (success, error) = await userService.UnrestrictAsync(id);
        if (!success)
            return BadRequest(new { error });

        return Ok(new { message = "User unrestricted successfully." });
    }
}
