using FluentValidation;
using InventoryApi.DTOs.User;

namespace InventoryApi.Validation
{
    public class UserRegisterDTOValidator : AbstractValidator<UserRegisterDTO>
    {
        public UserRegisterDTOValidator()
        {
            RuleFor(u => u.Username)
                .NotEmpty()
                .WithMessage($"{nameof(UserRegisterDTO.Username)} is empty.")
                .NotNull()
                .WithMessage($"{nameof(UserRegisterDTO.Username)} is required.")
                .MaximumLength(50);

            RuleFor(u => u.Email)
                .NotEmpty()
                .WithMessage($"{nameof(UserRegisterDTO.Email)} is empty.")
                .NotNull()
                .WithMessage($"{nameof(UserRegisterDTO.Email)} is required.")
                .EmailAddress()
                .WithMessage($"{nameof(UserRegisterDTO.Email)} is not an email.");

            RuleFor(u => u.Password)
                .NotEmpty()
                .WithMessage($"{nameof(UserRegisterDTO.Password)} is empty.")
                .NotNull()
                .WithMessage($"{nameof(UserRegisterDTO.Password)} is required.")
                .MaximumLength(50);

            RuleFor(u => u.FirstName)
                .NotEmpty()
                .WithMessage($"{nameof(UserRegisterDTO.FirstName)} is empty.")
                .NotNull()
                .WithMessage($"{nameof(UserRegisterDTO.FirstName)} is required.")
                .MaximumLength(50);

            RuleFor(u => u.LastName)
                .NotEmpty()
                .WithMessage($"{nameof(UserRegisterDTO.LastName)} is empty.")
                .NotNull()
                .WithMessage($"{nameof(UserRegisterDTO.LastName)} is required.")
                .MaximumLength(50);

        }
    }
}
