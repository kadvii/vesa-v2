namespace Vesa.DTOs.Dashboard;

public class DashboardOverviewResponse
{
    public int TotalApplications { get; set; }
    public int PendingCount { get; set; }
    public int ApprovedToday { get; set; }
    public decimal RevenueThisMonth { get; set; }
    public int AppointmentsToday { get; set; }
}
