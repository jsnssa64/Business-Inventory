using Domain.ValueObjects.Inventory;

namespace Domain.Entities.Inventory
{
    public class InventoryItem
    {
        public InventoryItemIdentity Id { get; set; } = new InventoryItemIdentity();
        public int Quantity { get; set; } = 0;

        public void UpdateQuantity(int newQuantity)
        {
            if (newQuantity < 0)
            {
                throw new ArgumentException("Quantity cannot be negative.");
            }
            Quantity = newQuantity;
        }
    }
}
