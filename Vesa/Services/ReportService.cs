using System;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Vesa.Data;
using Vesa.Models.Enums;
using Vesa.Services.Interfaces;

namespace Vesa.Services;

public class ReportService(AppDbContext db) : IReportService
{
    public async Task<byte[]> ExportApplicationsCsvAsync(VisaApplicationStatus? status, Guid? countryId)
    {
        var query = db.VisaApplications
            .Include(a => a.Applicant)
            .Include(a => a.VisaType)
            .Include(a => a.Country)
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(a => a.Status == status.Value);

        if (countryId.HasValue)
            query = query.Where(a => a.CountryId == countryId.Value);

        var apps = await query.OrderByDescending(a => a.SubmittedAt).ToListAsync();

        var sb = new StringBuilder();
        sb.AppendLine("Id,ApplicantName,PassportNumber,Country,VisaType,Status,SubmittedAt,ReviewedAt");

        foreach (var a in apps)
        {
            var id = a.Id.ToString();
            var applicantName = EscapeCsvField(a.Applicant?.FullName);
            var passport = EscapeCsvField(a.PassportNumber);
            var country = EscapeCsvField(a.Country?.Name);
            var visaType = EscapeCsvField(a.VisaType?.Name);
            var appStatus = a.Status.ToString();
            var submitted = a.SubmittedAt.ToString("yyyy-MM-dd HH:mm:ss");
            var reviewed = a.ReviewedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? string.Empty;

            sb.AppendLine($"{id},{applicantName},{passport},{country},{visaType},{appStatus},{submitted},{reviewed}");
        }

        var bom = Encoding.UTF8.GetPreamble();
        var content = Encoding.UTF8.GetBytes(sb.ToString());
        return [.. bom, .. content];
    }

    public async Task<byte[]> ExportPaymentsCsvAsync(PaymentStatus? status, DateTime? from, DateTime? to)
    {
        var query = db.Payments
            .Include(p => p.Applicant)
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(p => p.Status == status.Value);

        if (from.HasValue)
            query = query.Where(p => p.PaidAt >= from.Value);

        if (to.HasValue)
            query = query.Where(p => p.PaidAt <= to.Value);

        var payments = await query.OrderByDescending(p => p.CreatedAt).ToListAsync();

        var sb = new StringBuilder();
        sb.AppendLine("Id,ApplicantName,ApplicationId,Amount,Status,Method,TransactionReference,PaidAt");

        foreach (var p in payments)
        {
            var id = p.Id.ToString();
            var applicantName = EscapeCsvField(p.Applicant?.FullName);
            var appId = p.ApplicationId.ToString();
            var amount = p.Amount.ToString("F2");
            var payStatus = p.Status.ToString();
            var method = p.Method?.ToString() ?? string.Empty;
            var reference = EscapeCsvField(p.TransactionReference);
            var paidAt = p.PaidAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? string.Empty;

            sb.AppendLine($"{id},{applicantName},{appId},{amount},{payStatus},{method},{reference},{paidAt}");
        }

        var bom = Encoding.UTF8.GetPreamble();
        var content = Encoding.UTF8.GetBytes(sb.ToString());
        return [.. bom, .. content];
    }

    private static string EscapeCsvField(string? field)
    {
        if (string.IsNullOrEmpty(field))
            return string.Empty;

        if (field.Contains(',') || field.Contains('"') || field.Contains('\n') || field.Contains('\r'))
        {
            return $"\"{field.Replace("\"", "\"\"")}\"";
        }

        return field;
    }
}
