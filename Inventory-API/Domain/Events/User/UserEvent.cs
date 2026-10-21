using Domain.ValueObjects.User;

namespace Domain.Events.User
{
    public record UserEvent(int Version, UserIdentity UserIdentity) : DomainEvent("User", Version);
}


