using Domain.ValueObjects.Product;

namespace Domain.ValueObjects.Inventory
{
    public readonly record struct InventoryItemIdentity(ProductIdentity productIdentity, Guid InventoryId);
}
