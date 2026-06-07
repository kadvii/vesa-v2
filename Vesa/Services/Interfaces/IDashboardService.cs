using System.Collections.Generic;
using System.Threading.Tasks;
using Vesa.DTOs.Applications;
using Vesa.DTOs.Dashboard;

namespace Vesa.Services.Interfaces;

public interface IDashboardService
{
    Task<DashboardOverviewResponse> GetOverviewAsync();
    Task<Dictionary<string, int>> GetApplicationsByStatusAsync();
    Task<Dictionary<string, int>> GetApplicationsByCountryAsync();
    Task<RevenueBreakdownResponse> GetRevenueBreakdownAsync();
    Task<IList<ApplicationResponse>> GetRecentApplicationsAsync();
}
