using Domain.Entities.User;
using FluentValidation;

namespace InventoryApi.Validation
{
    public class UserDetailValidator : AbstractValidator<UserDetails>
    {
        public UserDetailValidator()
        {
            RuleFor(u => u.FirstName)
                .NotEmpty()
                .WithMessage($"{nameof(UserDetails.FirstName)} is empty.")
                .NotNull()
                .WithMessage($"{nameof(UserDetails.FirstName)} is required.")
                .MaximumLength(50);

            RuleFor(u => u.LastName)
                .NotEmpty()
                .WithMessage($"{nameof(UserDetails.LastName)} is empty.")
                .NotNull()
                .WithMessage($"{nameof(UserDetails.LastName)} is required.")
                .MaximumLength(50);

            RuleFor(u => u.DOB)
                .NotNull()
                .Must(dob => dob < DateOnly.FromDateTime(DateTime.Now))
                .WithMessage("Date of Birth must be in the past.")
                .When(u => u.DOB is not null);

            RuleFor(u => u.ContactNumber)
                .NotEmpty()
                .WithMessage($"{nameof(UserDetails.ContactNumber)} is required.")
                .MaximumLength(50)
                .When(u => u.ContactNumber is not null);

            RuleFor(u => u.Gender)
                .NotEmpty()
                .WithMessage($"{nameof(UserDetails.Gender)} is required.")
                .MaximumLength(50)
                .When(u => u.Gender is not null);

            RuleFor(u => u.userAddress)
                .NotNull()
                .SetValidator(new AddressValidator() as IValidator<UserAddress?>)
                .When(u => u.userAddress is not null);
        }
    }
}
