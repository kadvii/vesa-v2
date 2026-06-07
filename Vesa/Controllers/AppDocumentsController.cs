using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vesa.Models.Enums;
using Vesa.Services.Interfaces;

namespace Vesa.Controllers;

[ApiController]
[Route("api/app/documents")]
[Authorize(Roles = "Applicant")]
public class AppDocumentsController(IDocumentService documentService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Upload(
        [FromForm] IFormFile file,
        [FromForm] Guid applicationId,
        [FromForm] DocumentType documentType)
    {
        var applicantId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(applicantId))
            return Unauthorized();

        var (success, data, error) = await documentService.UploadAsync(file, applicationId, documentType, applicantId, isAdmin: false);
        if (!success)
            return BadRequest(new { error });

        return Ok(data);
    }

    [HttpGet("application/{applicationId:guid}")]
    public async Task<IActionResult> GetByApplication(Guid applicationId)
    {
        var applicantId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(applicantId))
            return Unauthorized();

        try
        {
            var documents = await documentService.GetByApplicationIdAsync(applicationId, applicantId, isAdmin: false);
            return Ok(documents);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }
}
