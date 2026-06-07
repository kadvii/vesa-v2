using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vesa.Services.Interfaces;

namespace Vesa.Controllers;

[ApiController]
[Route("api/app/notifications")]
[Authorize(Roles = "Applicant")]
public class AppNotificationsController(INotificationService notificationService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetMine()
    {
        var applicantId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(applicantId))
            return Unauthorized();

        var notifications = await notificationService.GetMyNotificationsAsync(applicantId);
        return Ok(notifications);
    }

    [HttpPost("{id:guid}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var applicantId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(applicantId))
            return Unauthorized();

        var (success, error) = await notificationService.MarkAsReadAsync(id, applicantId);
        if (!success)
            return BadRequest(new { error });

        return Ok(new { message = "Notification marked as read." });
    }
}
