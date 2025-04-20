namespace InventoryApi.Repository.Data.Product
{
    public class ProductIdentifierModel
    {
        public required string Username { get; set; }
        public required Guid PublicProductId { get; set; }
    }
}
