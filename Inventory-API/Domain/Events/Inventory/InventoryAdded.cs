using Domain.Entities.Inventory;

namespace Domain.Events.Inventory
{
    public record InventoryAdded(int Version, InventoryItemIdentity InventoryItemIdentity, int Quantity) : InventoryEvent(Version, InventoryItemIdentity);
}
