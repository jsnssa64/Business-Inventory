namespace InventoryApi.Repository.Data.Product
{
    public class UpdateProductDetailsModel
    {
        public string? Name { get; set; } = null;
        public string? Description { get; set; } = null;
        public int? Quantity { get; set; } = null;
        public bool? EnabledPrice { get; set; } = null;
    }
}
