using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vesa.DTOs.Payments;
using Vesa.Services.Interfaces;

namespace Vesa.Controllers;

[ApiController]
[Route("api/app/payments")]
[Authorize(Roles = "Applicant")]
public class AppPaymentsController(IPaymentService paymentService) : ControllerBase
{
    [HttpGet("application/{applicationId:guid}")]
    public async Task<IActionResult> GetByApplicationId(Guid applicationId)
    {
        var applicantId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(applicantId))
            return Unauthorized();

        try
        {
            var payment = await paymentService.GetByApplicationIdAsync(applicationId, applicantId, isAdmin: false);
            if (payment is null)
                return NotFound(new { error = "Payment not found for this application." });

            return Ok(payment);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpPost("{id:guid}/confirm")]
    public async Task<IActionResult> Confirm(Guid id, [FromBody] ConfirmPaymentRequest request)
    {
        var applicantId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(applicantId))
            return Unauthorized();

        var (success, data, error) = await paymentService.ConfirmPaymentAsync(id, request, applicantId, isAdmin: false);
        if (!success)
            return BadRequest(new { error });

        return Ok(data);
    }
}
