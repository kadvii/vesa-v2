using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vesa.DTOs.Applications;
using Vesa.Services.Interfaces;

namespace Vesa.Controllers;

[ApiController]
[Route("api/app/applications")]
[Authorize(Roles = "Applicant")]
public class AppApplicationsController(IVisaApplicationService applicationService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Submit([FromBody] SubmitApplicationRequest request)
    {
        var applicantId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(applicantId))
            return Unauthorized();

        var (success, data, error) = await applicationService.SubmitAsync(request, applicantId);
        if (!success)
            return BadRequest(new { error });

        return Ok(data);
    }

    [HttpGet]
    public async Task<IActionResult> GetMyApplications()
    {
        var applicantId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(applicantId))
            return Unauthorized();

        var applications = await applicationService.GetMyApplicationsAsync(applicantId);
        return Ok(applications);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var applicantId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(applicantId))
            return Unauthorized();

        try
        {
            var application = await applicationService.GetByIdAsync(id, applicantId, isAdmin: false);
            if (application is null)
                return NotFound();

            return Ok(application);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var applicantId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(applicantId))
            return Unauthorized();

        var (success, error) = await applicationService.CancelAsync(id, applicantId);
        if (!success)
            return BadRequest(new { error });

        return Ok(new { message = "Application cancelled successfully." });
    }

    [HttpGet("{id:guid}/timeline")]
    public async Task<IActionResult> GetTimeline(Guid id)
    {
        var applicantId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(applicantId))
            return Unauthorized();

        try
        {
            var application = await applicationService.GetByIdAsync(id, applicantId, isAdmin: false);
            if (application is null)
                return NotFound();

            var timeline = await applicationService.GetStatusHistoryAsync(id);
            return Ok(timeline);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }
}
