using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vesa.DTOs.Payments;
using Vesa.Models.Enums;
using Vesa.Services.Interfaces;

namespace Vesa.Controllers;

[ApiController]
[Route("api/admin/payments")]
[Authorize(Roles = "Admin")]
public class AdminPaymentsController(IPaymentService paymentService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] PaymentStatus? status,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        var payments = await paymentService.GetAllAsync(status, startDate, endDate);
        return Ok(payments);
    }

    [HttpPost("{id:guid}/confirm")]
    public async Task<IActionResult> Confirm(Guid id, [FromBody] ConfirmPaymentRequest request)
    {
        // For admin confirmations, we pass an empty/dummy string for applicantId since isAdmin is true
        var (success, data, error) = await paymentService.ConfirmPaymentAsync(id, request, string.Empty, isAdmin: true);
        if (!success)
            return BadRequest(new { error });

        return Ok(data);
    }

    [HttpPost("{id:guid}/refund")]
    public async Task<IActionResult> Refund(Guid id)
    {
        var (success, data, error) = await paymentService.RefundAsync(id);
        if (!success)
            return BadRequest(new { error });

        return Ok(data);
    }
}
