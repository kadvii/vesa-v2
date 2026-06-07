using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Vesa.Data;
using Vesa.DTOs.Documents;
using Vesa.Models;
using Vesa.Models.Enums;
using Vesa.Services.Interfaces;

namespace Vesa.Services;

public class DocumentService(AppDbContext db) : IDocumentService
{
    private static readonly HashSet<string> AllowedExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".pdf", ".jpg", ".jpeg", ".png" };

    private const long MaxFileSizeBytes = 5 * 1024 * 1024;

    private static readonly Dictionary<string, byte[][]> MagicBytes = new(StringComparer.OrdinalIgnoreCase)
    {
        [".jpg"]  = [new byte[] { 0xFF, 0xD8, 0xFF }],
        [".jpeg"] = [new byte[] { 0xFF, 0xD8, 0xFF }],
        [".png"]  = [new byte[] { 0x89, 0x50, 0x4E, 0x47 }],
        [".pdf"]  = [new byte[] { 0x25, 0x50, 0x44, 0x46 }],
    };

    private readonly string _uploadsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "uploads");

    public async Task<(bool success, DocumentResponse? data, string? error)> UploadAsync(
        IFormFile file, Guid applicationId, DocumentType documentType, string requestingUserId, bool isAdmin)
    {
        if (file is null || file.Length == 0)
            return (false, null, "No file uploaded.");

        if (file.Length > MaxFileSizeBytes)
            return (false, null, "File size exceeds 5 MB limit.");

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(ext))
            return (false, null, "File format not supported. Only PDF, JPG, and PNG are allowed.");

        if (!await HasValidMagicBytesAsync(file, ext))
            return (false, null, "File verification failed. Content does not match extension.");

        var app = await db.VisaApplications.FindAsync(applicationId);
        if (app is null)
            return (false, null, "Visa application not found.");

        if (!isAdmin && app.ApplicantId != requestingUserId)
            return (false, null, "You do not have permission to upload documents for this application.");

        Directory.CreateDirectory(_uploadsPath);

        var safeOriginalName = Path.GetFileName(file.FileName);
        var uniqueName = $"{Guid.NewGuid()}{ext}";
        var fullPath = Path.Combine(_uploadsPath, uniqueName);

        await using (var stream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, 81920, useAsync: true))
        {
            await file.CopyToAsync(stream);
        }

        var document = new Document
        {
            Id = Guid.NewGuid(),
            ApplicationId = applicationId,
            FileName = safeOriginalName,
            FileUrl = $"/uploads/{uniqueName}",
            DocumentType = documentType,
            UploadedAt = DateTime.UtcNow
        };

        db.Documents.Add(document);
        await db.SaveChangesAsync();

        return (true, ToResponse(document), null);
    }

    public async Task<IList<DocumentResponse>> GetByApplicationIdAsync(Guid applicationId, string userId, bool isAdmin)
    {
        var app = await db.VisaApplications.FindAsync(applicationId);
        if (app is null)
            return [];

        if (!isAdmin && app.ApplicantId != userId)
            throw new UnauthorizedAccessException("You are not authorized to view documents for this application.");

        var docs = await db.Documents
            .Where(d => d.ApplicationId == applicationId)
            .OrderByDescending(d => d.UploadedAt)
            .ToListAsync();

        return docs.Select(ToResponse).ToList();
    }

    private static async Task<bool> HasValidMagicBytesAsync(IFormFile file, string ext)
    {
        if (!MagicBytes.TryGetValue(ext, out var signatures))
            return false;

        var maxHeader = signatures.Max(s => s.Length);
        var header = new byte[maxHeader];

        await using var stream = file.OpenReadStream();
        var bytesRead = await stream.ReadAsync(header.AsMemory(0, maxHeader));

        return signatures.Any(sig =>
            bytesRead >= sig.Length &&
            header.Take(sig.Length).SequenceEqual(sig));
    }

    private static DocumentResponse ToResponse(Document d) => new()
    {
        Id = d.Id,
        ApplicationId = d.ApplicationId,
        FileName = d.FileName,
        FileUrl = d.FileUrl,
        DocumentType = d.DocumentType.ToString(),
        UploadedAt = d.UploadedAt
    };
}
