using Domain.Entities.User;
using FluentValidation;

namespace InventoryApi.Validation
{
    public class UserValidator: AbstractValidator<UserLogin>
    {
        public UserValidator()
        {
            RuleFor(u => u.Username)
                .NotEmpty()
                .WithMessage($"{nameof(UserLogin.Username)} is empty.")
                .NotNull()
                .WithMessage($"{nameof(UserLogin.Username)} is required.")
                .MaximumLength(50);

            //RuleFor(u => u.Email)
            //    .NotEmpty()
            //    .WithMessage($"{nameof(UserLogin.Email)} is empty.")
            //    .NotNull()
            //    .WithMessage($"{nameof(UserLogin.Email)} is required.")
            //    .EmailAddress()
            //    .WithMessage($"{nameof(UserLogin.Email)} is not an email.");

            RuleFor(u => u.Password)
                .NotEmpty()
                .WithMessage($"{nameof(UserLogin.Password)} is required.")
                .NotNull()
                .WithMessage($"{nameof(UserLogin.Password)} is required.");
        }
    }
}
