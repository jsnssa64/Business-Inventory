namespace Domain.Inventory
{
    public class InventoryItem
    {
        public Guid PublicProductId { get; set; }
        public string? ProductName { get; set; }
        public int InventoryQuantity { get; set; }
    }
}
