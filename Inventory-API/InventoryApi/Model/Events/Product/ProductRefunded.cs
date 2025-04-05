namespace InventoryApi.Model.Events.Product
{
    public class ProductRefunded: ProductAction
    {
        public required string UserId { get; set; }
        public int Quantity { get; set; }
    }
}
