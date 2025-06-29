using Domain.Entities.Product;

namespace Domain.Entities.Inventory
{
    public class InventoryItem
    {
        public InventoryItemIdentity InventoryItemIdentity { get; set; } = new InventoryItemIdentity();
        public int Quantity { get; set; } = 0;
    }
}
