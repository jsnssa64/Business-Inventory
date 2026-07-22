using Domain.Entities.Product;

namespace Domain.Entities.Inventory
{
    public readonly record struct InventoryItemIdentity(ProductIdentity productIdentity, Guid InventoryId);
}
