using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Vesa.Data;
using Vesa.DTOs.Applications;
using Vesa.DTOs.Dashboard;
using Vesa.Models;
using Vesa.Models.Enums;
using Vesa.Services.Interfaces;

namespace Vesa.Services;

public class DashboardService(AppDbContext db) : IDashboardService
{
    public async Task<DashboardOverviewResponse> GetOverviewAsync()
    {
        var now = DateTime.UtcNow;
        var todayStart = now.Date;
        var firstDayOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var todayDate = DateOnly.FromDateTime(now);

        var totalApplications = await db.VisaApplications.CountAsync();

        var pendingCount = await db.VisaApplications
            .CountAsync(a => a.Status == VisaApplicationStatus.Submitted || a.Status == VisaApplicationStatus.UnderReview);

        var approvedToday = await db.VisaApplications
            .CountAsync(a => a.Status == VisaApplicationStatus.Approved && a.ReviewedAt >= todayStart);

        var revenueThisMonth = await db.Payments
            .Where(p => p.Status == PaymentStatus.Paid && p.PaidAt >= firstDayOfMonth)
            .SumAsync(p => p.Amount);

        var appointmentsToday = await db.Appointments
            .CountAsync(a => a.AppointmentSlot.Date == todayDate && a.Status != AppointmentStatus.Cancelled);

        return new DashboardOverviewResponse
        {
            TotalApplications = totalApplications,
            PendingCount = pendingCount,
            ApprovedToday = approvedToday,
            RevenueThisMonth = revenueThisMonth,
            AppointmentsToday = appointmentsToday
        };
    }

    public async Task<Dictionary<string, int>> GetApplicationsByStatusAsync()
    {
        var groups = await db.VisaApplications
            .GroupBy(a => a.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync();

        // Initialize dictionary with all statuses set to 0
        var result = Enum.GetValues<VisaApplicationStatus>()
            .ToDictionary(s => s.ToString(), _ => 0);

        foreach (var group in groups)
        {
            result[group.Status.ToString()] = group.Count;
        }

        return result;
    }

    public async Task<Dictionary<string, int>> GetApplicationsByCountryAsync()
    {
        var groups = await db.VisaApplications
            .Include(a => a.Country)
            .GroupBy(a => a.Country.Name)
            .Select(g => new { Country = g.Key, Count = g.Count() })
            .ToListAsync();

        return groups.ToDictionary(g => g.Country, g => g.Count);
    }

    public async Task<RevenueBreakdownResponse> GetRevenueBreakdownAsync()
    {
        var now = DateTime.UtcNow;
        var thirtyDaysAgo = now.AddDays(-30).Date;
        var twelveMonthsAgo = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(-11);

        // Daily breakdown
        var dailyRaw = await db.Payments
            .Where(p => p.Status == PaymentStatus.Paid && p.PaidAt >= thirtyDaysAgo)
            .GroupBy(p => p.PaidAt!.Value.Date)
            .Select(g => new { Date = g.Key, Amount = g.Sum(p => p.Amount) })
            .ToListAsync();

        var dailyRevenue = dailyRaw
            .OrderBy(d => d.Date)
            .Select(d => new DailyRevenueData
            {
                Date = d.Date.ToString("yyyy-MM-dd"),
                Amount = d.Amount
            })
            .ToList();

        // Monthly breakdown
        var monthlyRaw = await db.Payments
            .Where(p => p.Status == PaymentStatus.Paid && p.PaidAt >= twelveMonthsAgo)
            .GroupBy(p => new { Year = p.PaidAt!.Value.Year, Month = p.PaidAt!.Value.Month })
            .Select(g => new { g.Key.Year, g.Key.Month, Amount = g.Sum(p => p.Amount) })
            .ToListAsync();

        var monthlyRevenue = monthlyRaw
            .OrderBy(m => m.Year)
            .ThenBy(m => m.Month)
            .Select(m => new MonthlyRevenueData
            {
                Month = $"{m.Year}-{m.Month:D2}",
                Amount = m.Amount
            })
            .ToList();

        return new RevenueBreakdownResponse
        {
            DailyRevenue = dailyRevenue,
            MonthlyRevenue = monthlyRevenue
        };
    }

    public async Task<IList<ApplicationResponse>> GetRecentApplicationsAsync()
    {
        var apps = await db.VisaApplications
            .Include(a => a.Applicant)
            .Include(a => a.VisaType)
            .Include(a => a.Country)
            .OrderByDescending(a => a.SubmittedAt)
            .Take(10)
            .ToListAsync();

        return apps.Select(ToApplicationResponse).ToList();
    }

    private static ApplicationResponse ToApplicationResponse(VisaApplication a) => new()
    {
        Id = a.Id,
        ApplicantId = a.ApplicantId,
        ApplicantName = a.Applicant?.FullName ?? string.Empty,
        VisaTypeId = a.VisaTypeId,
        VisaTypeName = a.VisaType?.Name ?? string.Empty,
        CountryId = a.CountryId,
        CountryName = a.Country?.Name ?? string.Empty,
        PassportNumber = a.PassportNumber,
        PassportExpiry = a.PassportExpiry,
        TravelDateFrom = a.TravelDateFrom,
        TravelDateTo = a.TravelDateTo,
        Status = a.Status.ToString(),
        AdminNotes = a.AdminNotes,
        RejectionReason = a.RejectionReason,
        SubmittedAt = a.SubmittedAt,
        ReviewedAt = a.ReviewedAt,
        ReviewedByAdminId = a.ReviewedByAdminId
    };
}
