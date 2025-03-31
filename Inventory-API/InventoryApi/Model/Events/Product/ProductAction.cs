namespace InventoryApi.Model.Events.Product
{
    public class ProductAction
    {
        public static string StreamName = "Product";
        public string ProductId { get; set; }
    }
}
