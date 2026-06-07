using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vesa.Services.Interfaces;

namespace Vesa.Controllers;

[ApiController]
[Route("api/admin/dashboard")]
[Authorize(Roles = "Admin")]
public class AdminDashboardController(IDashboardService dashboardService) : ControllerBase
{
    [HttpGet("overview")]
    public async Task<IActionResult> GetOverview()
    {
        var overview = await dashboardService.GetOverviewAsync();
        return Ok(overview);
    }

    [HttpGet("applications-by-status")]
    public async Task<IActionResult> GetApplicationsByStatus()
    {
        var stats = await dashboardService.GetApplicationsByStatusAsync();
        return Ok(stats);
    }

    [HttpGet("applications-by-country")]
    public async Task<IActionResult> GetApplicationsByCountry()
    {
        var stats = await dashboardService.GetApplicationsByCountryAsync();
        return Ok(stats);
    }

    [HttpGet("revenue")]
    public async Task<IActionResult> GetRevenue()
    {
        var breakdown = await dashboardService.GetRevenueBreakdownAsync();
        return Ok(breakdown);
    }

    [HttpGet("recent-applications")]
    public async Task<IActionResult> GetRecentApplications()
    {
        var applications = await dashboardService.GetRecentApplicationsAsync();
        return Ok(applications);
    }
}
