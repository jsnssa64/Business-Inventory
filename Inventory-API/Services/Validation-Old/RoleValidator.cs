using Domain.Entities.User;
using Domain.ValueObjects.User.Role;
using FluentValidation;

namespace InventoryApi.Validation
{
    public class RoleValidator: AbstractValidator<UserRoles>
    {
        public RoleValidator()
        {
            RuleFor(u => u.UserRole)
                .NotEmpty()
                .WithMessage($"{nameof(UserRoles.UserRole)} is empty.")
                .NotNull()
                .WithMessage($"{nameof(UserRoles.UserRole)} is required.");
        }
    }
}
