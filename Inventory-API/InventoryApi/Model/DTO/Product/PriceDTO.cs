namespace InventoryApi.Model.DTO.Product
{
    public class UpdatePriceDTO
    {
        public decimal? Price { get; private set; } = null;
        public string? CurrencyCode { get; set; } = null;
    }
}
