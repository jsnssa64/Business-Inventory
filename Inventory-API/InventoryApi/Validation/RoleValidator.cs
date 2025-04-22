using Domain.User;
using FluentValidation;

namespace InventoryApi.Validation
{
    public class RoleValidator: AbstractValidator<UserRole>
    {
        public RoleValidator()
        {
            RuleFor(u => u.Rolename)
                .NotEmpty()
                .WithMessage($"{nameof(UserRole.Rolename)} is empty.")
                .NotNull()
                .WithMessage($"{nameof(UserRole.Rolename)} is required.");
        }
    }
}
