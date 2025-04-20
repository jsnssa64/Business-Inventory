namespace InventoryApi.Repository.Data
{
    public class PriceModel
    {
        public decimal Price {  get; set; }
        public required string CurrencyCode { get; set; }
    }

    public class UpdatePriceModel
    {
        public decimal? Price { get; set; } = null; 
        public string? CurrencyCode { get; set; } = null;
    }
}
