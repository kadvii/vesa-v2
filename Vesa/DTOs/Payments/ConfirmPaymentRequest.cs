using Vesa.Models.Enums;

namespace Vesa.DTOs.Payments;

public class ConfirmPaymentRequest
{
    public PaymentMethod Method { get; set; }
    public string? TransactionReference { get; set; }
}
