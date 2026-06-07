using System.Collections.Generic;

namespace Vesa.DTOs.Dashboard;

public class DailyRevenueData
{
    public string Date { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

public class MonthlyRevenueData
{
    public string Month { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

public class RevenueBreakdownResponse
{
    public List<DailyRevenueData> DailyRevenue { get; set; } = [];
    public List<MonthlyRevenueData> MonthlyRevenue { get; set; } = [];
}
