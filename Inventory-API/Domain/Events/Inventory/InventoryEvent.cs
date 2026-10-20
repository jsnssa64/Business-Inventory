using Domain.Entities.Inventory;

namespace Domain.Events.Inventory
{
    public record InventoryEvent(int Version, InventoryItemIdentity InventoryItemIdentity) : DomainEvent("Inventory", Version);
}
