namespace InventoryApi.Repository.Data.Product
{
    public class ProductDetailsModel
    {
        public string? Name { get; set; } = null;
        public string? Description { get; set; } = null;
        public int Quantity { get; set; } = 0;
        public string? ProductName { get; set; } = null;
        public bool? EnabledPrice { get; set; } = null;
        public int InventoryQuantity { get; set; } = 0;
    }
}
