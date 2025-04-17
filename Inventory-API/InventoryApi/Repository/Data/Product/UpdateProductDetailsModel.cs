namespace InventoryApi.Repository.Data.Product
{
    public class UpdateProductDetailsModel
    {
        public string? Name { get; set; } = null;
        public string? Description { get; set; } = null;
        public int Quantity { get; set; } = 0;
        public PriceModel? Price { get; set; } = null;
    }
}
