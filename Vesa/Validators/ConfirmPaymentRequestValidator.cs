using FluentValidation;
using Vesa.DTOs.Payments;
using Vesa.Models.Enums;

namespace Vesa.Validators;

public class ConfirmPaymentRequestValidator : AbstractValidator<ConfirmPaymentRequest>
{
    public ConfirmPaymentRequestValidator()
    {
        RuleFor(x => x.Method)
            .IsInEnum().WithMessage("Invalid payment method.");

        RuleFor(x => x.TransactionReference)
            .NotEmpty()
            .When(x => x.Method == PaymentMethod.QiCard || x.Method == PaymentMethod.BankTransfer)
            .WithMessage("Transaction Reference is required for electronic payments.");
    }
}
