namespace InventoryApi.Repository.Data
{
    public class UpdatePriceModel
    {
        public decimal? Price { get; set; } = null; 
        public string? CurrencyCode { get; set; } = null;
    }
}
