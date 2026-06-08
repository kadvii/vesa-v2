using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Vesa.Data;
using Vesa.DTOs.Applications;
using Vesa.DTOs.Users;
using Vesa.Models;
using Vesa.Services.Interfaces;

namespace Vesa.Services;

public class UserService(
    UserManager<AppUser> userManager,
    AppDbContext db) : IUserService
{
    public async Task<IList<UserResponse>> GetAllAsync(string? search)
    {
        var query = userManager.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(u =>
                u.FullName.Contains(search) ||
                u.Email!.Contains(search) ||
                u.NationalId.Contains(search));
        }

        var users = await query.ToListAsync();

        var appCounts = await db.VisaApplications
            .GroupBy(a => a.ApplicantId)
            .Select(g => new { ApplicantId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.ApplicantId, x => x.Count);

        return users.Select(u => new UserResponse
        {
            Id = u.Id,
            FullName = u.FullName,
            Email = u.Email ?? string.Empty,
            PhoneNumber = u.PhoneNumber ?? string.Empty,
            NationalId = u.NationalId,
            DateOfBirth = u.DateOfBirth,
            CreatedAt = u.CreatedAt,
            IsRestricted = u.IsRestricted,
            TotalApplications = appCounts.TryGetValue(u.Id, out var count) ? count : 0
        }).ToList();
    }

    public async Task<UserDetailsResponse?> GetByIdAsync(string userId)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null)
            return null;

        var apps = await db.VisaApplications
            .Include(a => a.Applicant)
            .Include(a => a.VisaType)
            .Include(a => a.Country)
            .Where(a => a.ApplicantId == userId)
            .OrderByDescending(a => a.SubmittedAt)
            .ToListAsync();

        var userApps = apps.Select(a => new ApplicationResponse
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
        }).ToList();

        return new UserDetailsResponse
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            PhoneNumber = user.PhoneNumber ?? string.Empty,
            NationalId = user.NationalId,
            DateOfBirth = user.DateOfBirth,
            CreatedAt = user.CreatedAt,
            IsRestricted = user.IsRestricted,
            TotalApplications = userApps.Count,
            Applications = userApps
        };
    }

    public async Task<(bool success, string? error)> RestrictAsync(string userId, string reason)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            return (false, "User not found.");

        user.IsRestricted = true;
        user.RestrictionReason = reason;

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errorMsg = string.Join(" ", result.Errors.Select(e => e.Description));
            return (false, errorMsg);
        }

        return (true, null);
    }

    public async Task<(bool success, string? error)> UnrestrictAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            return (false, "User not found.");

        user.IsRestricted = false;
        user.RestrictionReason = null;

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errorMsg = string.Join(" ", result.Errors.Select(e => e.Description));
            return (false, errorMsg);
        }

        return (true, null);
    }
}
