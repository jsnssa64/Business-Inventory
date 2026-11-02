using Domain.ValueObjects.User;

namespace Domain.Events.User.Core
{
    public record UserEvent(int Version, UserIdentity UserIdentity) : DomainEvent(EventType.User, Version);
}


