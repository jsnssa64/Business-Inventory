using Domain.Entities.Inventory;

namespace Domain.Events.Inventory
{
    public record InventoryRemoved(int Version, InventoryItemIdentity InventoryItemIdentity, int Quantity) : InventoryEvent(Version, InventoryItemIdentity);
}
