using FluentValidation;
using Vesa.DTOs.Auth;

namespace Vesa.Validators;

public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
{
    public UpdateProfileRequestValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full Name is required.")
            .MaximumLength(100).WithMessage("Full Name cannot exceed 100 characters.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone Number is required.")
            .MaximumLength(20).WithMessage("Phone Number cannot exceed 20 characters.");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of Birth is required.")
            .Must(dob => dob < DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Date of Birth must be in the past.");
    }
}
