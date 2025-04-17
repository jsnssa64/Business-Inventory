namespace InventoryApi.Repository.Data
{
    public class PriceModel
    {
        public decimal Price {  get; set; }
        public required string CurrencyCode { get; set; }
    }
}
