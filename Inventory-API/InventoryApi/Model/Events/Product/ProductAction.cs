namespace InventoryApi.Model.Events.Product
{
    public class ProductAction
    {
        public static string StreamName = "Product";
        public required string ProductId { get; set; }
    }
}
