namespace InventoryApi.Model.DTO.Product
{
    public class ProductDTO
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public decimal Price { get; set; }
        public required string Currency { get; set; }
        public int ItemQuantity { get; set; }
        public int InventoryQuantity { get; set; }
        public bool? EnabledPrice { get; set; }
    }
}
