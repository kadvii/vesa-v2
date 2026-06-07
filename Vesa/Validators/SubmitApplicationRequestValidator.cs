using FluentValidation;
using Vesa.DTOs.Applications;

namespace Vesa.Validators;

public class SubmitApplicationRequestValidator : AbstractValidator<SubmitApplicationRequest>
{
    public SubmitApplicationRequestValidator()
    {
        RuleFor(x => x.VisaTypeId)
            .NotEmpty().WithMessage("Visa Type is required.");

        RuleFor(x => x.CountryId)
            .NotEmpty().WithMessage("Country is required.");

        RuleFor(x => x.PassportNumber)
            .NotEmpty().WithMessage("Passport Number is required.")
            .MaximumLength(50).WithMessage("Passport Number cannot exceed 50 characters.");

        RuleFor(x => x.PassportExpiry)
            .NotEmpty().WithMessage("Passport Expiry date is required.")
            .Must(expiry => expiry > DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Passport must be valid and not expired.");

        RuleFor(x => x.TravelDateFrom)
            .NotEmpty().WithMessage("Travel Start Date is required.")
            .Must(date => date >= DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Travel Start Date cannot be in the past.");

        RuleFor(x => x.TravelDateTo)
            .NotEmpty().WithMessage("Travel End Date is required.")
            .Must((request, dateTo) => dateTo > request.TravelDateFrom)
            .WithMessage("Travel End Date must be after the Start Date.");
    }
}
