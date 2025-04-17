namespace Domain.Inventory
{
    public class InventoryItem
    {
        public int Id { get; set; }
        public string? ProductId { get; set; }
        public int ProductName { get; set; }
        public int InventoryQuantity { get; set; }
    }
}
