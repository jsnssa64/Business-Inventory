using FluentValidation;
using Services.Service.User.Input;

namespace Services.Service.User.Validators;

public class UserIdInputValidator : AbstractValidator<UserIdInput>
{
    public UserIdInputValidator()
    {
        // TODO: Name shape rules (required? max length - from where?)
        // TODO: Description shape rules (required? max length?)
        // TODO: Price + Currency "both or neither" - shape rule here, or business rule in the domain?
        // TODO: Currency format (e.g. 3-letter ISO code) - shape here or Price value object?
        // TODO: user identity rule, once the input's user field is decided
    }
}
