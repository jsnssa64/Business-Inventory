using Domain.Entities.User;
using FluentValidation;

namespace InventoryApi.Validation
{
    public class AddressValidator : AbstractValidator<UserAddress>
    {
        public AddressValidator()
        {
            RuleFor(a => a.FirstLineAddress)
                .NotEmpty()
                .WithMessage($"First line of address is empty.")
                .NotNull()
                .WithMessage($"First line of address is required.");

            RuleFor(a => a.SecondLineAddress)
                .NotEmpty()
                .WithMessage("Second Line of address is empty.")
                .When(address => address is not null);

            RuleFor(a => a.Country)
                .NotEmpty()
                .WithMessage($"{nameof(UserAddress.Country)} is empty.")
                .NotNull()
                .WithMessage($"{nameof(UserAddress.Country)} is required.");

            RuleFor(a => a.PostCode)
                .NotEmpty()
                .WithMessage($"{nameof(UserAddress.PostCode)} is empty.")
                .NotNull()
                .WithMessage($"{nameof(UserAddress.PostCode)} is required.")
                .Length(5, 10);
        }
    }
}
