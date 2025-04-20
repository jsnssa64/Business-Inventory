namespace InventoryApi.Model.DTO.Product
{
    public class UpdateProductDTO
    {
        public required Guid ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? Description { get; set; }
        public int Quantity { get; set; }
        public PriceDTO? Price {  get; set; }
    }
}
