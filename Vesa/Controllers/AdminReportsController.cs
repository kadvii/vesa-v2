using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vesa.Models.Enums;
using Vesa.Services.Interfaces;

namespace Vesa.Controllers;

[ApiController]
[Route("api/admin/reports")]
[Authorize(Roles = "Admin")]
public class AdminReportsController(IReportService reportService) : ControllerBase
{
    [HttpGet("applications")]
    public async Task<IActionResult> ExportApplications(
        [FromQuery] VisaApplicationStatus? status,
        [FromQuery] Guid? countryId)
    {
        var bytes = await reportService.ExportApplicationsCsvAsync(status, countryId);
        var fileName = $"applications-{DateTime.UtcNow:yyyyMMdd}.csv";
        return File(bytes, "text/csv", fileName);
    }

    [HttpGet("payments")]
    public async Task<IActionResult> ExportPayments(
        [FromQuery] PaymentStatus? status,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to)
    {
        var bytes = await reportService.ExportPaymentsCsvAsync(status, from, to);
        var fileName = $"payments-{DateTime.UtcNow:yyyyMMdd}.csv";
        return File(bytes, "text/csv", fileName);
    }
}
