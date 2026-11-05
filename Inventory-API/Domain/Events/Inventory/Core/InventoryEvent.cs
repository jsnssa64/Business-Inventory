using Domain.ValueObjects.Inventory;

namespace Domain.Events.Inventory.Core
{
    public record InventoryEvent(int Version, InventoryItemIdentity InventoryItemIdentity) : DomainEvent(EventType.Inventory, Version);
}
