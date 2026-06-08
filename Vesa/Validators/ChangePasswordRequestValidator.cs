using FluentValidation;
using Vesa.DTOs.Auth;

namespace Vesa.Validators;

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("Current Password is required.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New Password is required.")
            .MinimumLength(8).WithMessage("New Password must be at least 8 characters long.");

        RuleFor(x => x.ConfirmNewPassword)
            .NotEmpty().WithMessage("Confirm New Password is required.")
            .Equal(x => x.NewPassword).WithMessage("Confirm New Password must match New Password.");
    }
}
