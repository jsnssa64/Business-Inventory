using Domain.Entities.Inventory;
using Domain.Entities.User;
using Domain.Events;

namespace Domain.Events.User
{
    public record UserEvent(int Version, UserIdentity UserIdentity) : DomainEvent("User", Version);
}


