namespace Services.DataModel.Product
{
    public class ProductDetailsModel
    {
        public required string ProductName { get; set; }
        public required string Description { get; set; }
        public int Quantity { get; set; } = 0;
        public bool EnabledPrice { get; set; } = false;
        public int InventoryQuantity { get; set; } = 0;
    }
}
