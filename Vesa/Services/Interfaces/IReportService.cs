using Vesa.Models.Enums;

namespace Vesa.Services.Interfaces;

public interface IReportService
{
    Task<byte[]> ExportApplicationsCsvAsync(VisaApplicationStatus? status, Guid? countryId);
    Task<byte[]> ExportPaymentsCsvAsync(PaymentStatus? status, DateTime? from, DateTime? to);
}
