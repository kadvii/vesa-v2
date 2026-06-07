using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vesa.DTOs.Payments;
using Vesa.Models.Enums;

namespace Vesa.Services.Interfaces;

public interface IPaymentService
{
    Task<PaymentResponse?> CreatePaymentAsync(Guid applicationId, string applicantId, decimal amount);
    Task<(bool success, PaymentResponse? data, string? error)> ConfirmPaymentAsync(Guid id, ConfirmPaymentRequest request, string userId, bool isAdmin);
    Task<PaymentResponse?> GetByApplicationIdAsync(Guid applicationId, string userId, bool isAdmin);
    Task<(bool success, PaymentResponse? data, string? error)> RefundAsync(Guid id);
    Task<IList<PaymentResponse>> GetAllAsync(PaymentStatus? status, DateTime? startDate, DateTime? endDate);
}
