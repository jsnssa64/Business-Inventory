namespace Domain.ValueObjects.Product
{
    public record struct ProductIdentity(ProductId PublicProductId, string Username, string Description);
}
