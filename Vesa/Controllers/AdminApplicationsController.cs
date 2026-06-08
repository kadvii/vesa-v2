using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vesa.DTOs.Applications;
using Vesa.Models.Enums;
using Vesa.Services.Interfaces;

namespace Vesa.Controllers;

[ApiController]
[Route("api/admin/applications")]
[Authorize(Roles = "Admin")]
public class AdminApplicationsController(
    IVisaApplicationService applicationService,
    IDocumentService documentService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] VisaApplicationStatus? status,
        [FromQuery] Guid? countryId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] string? search)
    {
        var applications = await applicationService.GetAllAsync(status, countryId, from, to, search);
        return Ok(applications);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var application = await applicationService.GetByIdAsync(id, string.Empty, isAdmin: true);
        if (application is null)
            return NotFound();

        return Ok(application);
    }

    [HttpPost("{id:guid}/status")]
    public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] ChangeStatusRequest request)
    {
        var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(adminId))
            return Unauthorized();

        var (success, error) = await applicationService.ChangeStatusAsync(id, request, adminId);
        if (!success)
            return BadRequest(new { error });

        return Ok(new { message = "Application status updated successfully." });
    }

    [HttpPost("{id:guid}/notes")]
    public async Task<IActionResult> AddNotes(Guid id, [FromBody] AddAdminNotesRequest request)
    {
        var (success, error) = await applicationService.AddAdminNotesAsync(id, request);
        if (!success)
            return BadRequest(new { error });

        return Ok(new { message = "Admin notes updated successfully." });
    }

    [HttpPost("{id:guid}/request-document")]
    public async Task<IActionResult> RequestDocument(Guid id, [FromBody] RequestDocumentRequest request)
    {
        var (success, error) = await applicationService.RequestDocumentAsync(id, request);
        if (!success)
            return BadRequest(new { error });

        return Ok(new { message = "Document request notification sent successfully." });
    }

    [HttpGet("{id:guid}/documents")]
    public async Task<IActionResult> GetDocuments(Guid id)
    {
        var documents = await documentService.GetByApplicationIdAsync(id, string.Empty, isAdmin: true);
        return Ok(documents);
    }

    [HttpGet("{id:guid}/timeline")]
    public async Task<IActionResult> GetTimeline(Guid id)
    {
        var application = await applicationService.GetByIdAsync(id, string.Empty, isAdmin: true);
        if (application is null)
            return NotFound();

        var timeline = await applicationService.GetStatusHistoryAsync(id);
        return Ok(timeline);
    }
}
