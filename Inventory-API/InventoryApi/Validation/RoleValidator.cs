using Domain.User;
using FluentValidation;

namespace InventoryApi.Validation
{
    public class RoleValidator: AbstractValidator<Role>
    {
        public RoleValidator()
        {
            RuleFor(u => u.Rolename)
                .NotEmpty()
                .WithMessage($"{nameof(Role.Rolename)} is empty.")
                .NotNull()
                .WithMessage($"{nameof(Role.Rolename)} is required.")
                .MaximumLength(50);
        }
    }
}
