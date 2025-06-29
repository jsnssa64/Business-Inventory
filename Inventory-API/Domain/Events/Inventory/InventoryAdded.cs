using Domain.Entities.Inventory;

namespace InventoryApi.Model.Events.Inventory
{
    public record InventoryAdded(int Version, InventoryItemIdentity InventoryItemIdentity, int Quantity) : InventoryEvent(Version, InventoryItemIdentity);
}
