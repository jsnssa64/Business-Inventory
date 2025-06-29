namespace Services.DataModel.Product
{
    public class ProductIdentifierModel
    {
        public required string Username { get; set; }
        public required Guid PublicProductId { get; set; }
    }
}
