namespace InventoryApi.DTOs.Product
{
    public class UpdateProductDTO
    {
        public required Guid ProductId { get; set; }
        public string? ProductName { get; set; } = null;
        public string? Description { get; set; } = null;
        public int Quantity { get; set; }
        public UpdatePriceDTO? Price {  get; set; } = null;
    }
}
