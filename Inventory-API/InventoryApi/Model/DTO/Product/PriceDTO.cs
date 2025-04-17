namespace InventoryApi.Model.DTO.Product
{
    public class PriceDTO
    {
        public decimal Price { get; private set; } = 0;
        public required string CurrencyCode { get; set; }
    }
}
