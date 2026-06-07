using System;
using Vesa.Models.Enums;

namespace Vesa.Models;

public class Payment
{
    public Guid Id { get; set; }
    public Guid ApplicationId { get; set; }
    public string ApplicantId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "IQD";
    public PaymentStatus Status { get; set; }
    public PaymentMethod? Method { get; set; }
    public string? TransactionReference { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public VisaApplication VisaApplication { get; set; } = null!;
    public AppUser Applicant { get; set; } = null!;
}
