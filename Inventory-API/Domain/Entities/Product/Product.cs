using Domain.ValueObjects.Product;

namespace Domain.Entities.Product
{
    public record Product(ProductIdentity Id, ProductInfo ProductInfo, bool Active, Price Price, int Version);
}
