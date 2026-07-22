using Domain.Entities.Inventory;
using InventoryApi.Model.Events.Product;

namespace InventoryApi.Model.Events.Inventory
{
    public record InventoryEvent(int Version, InventoryItemIdentity InventoryItemIdentity) : DomainEvent("Inventory", Version);
}
