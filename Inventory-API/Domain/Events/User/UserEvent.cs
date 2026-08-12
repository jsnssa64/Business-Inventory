using Domain.Entities.Inventory;
using Domain.ValueObjects.User;
using InventoryApi.Model.Events;

namespace InventoryApi.Service.UserService
{
    public record UserEvent(int Version, UserIdentity UserIdentity) : DomainEvent("User", Version);
}


