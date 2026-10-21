using Domain.Entities.Product;
using Domain.Events.Product.Enum;
using Domain.ValueObjects.Product;

namespace Domain.Events.Product
{
    public record ProductPriceEnabled(
        int Version, 
        ProductIdentity ProductIdentity,
        Price Price)
        : ProductEvent(
            Version, 
            ProductIdentity,
            ProductStatus.Active);
}
