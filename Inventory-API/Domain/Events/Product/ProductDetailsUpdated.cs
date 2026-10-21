using Domain.Events.Product.Enum;
using Domain.ValueObjects.Product;

namespace Domain.Events.Product
{
    public record ProductDetailsUpdated(
        int Version, 
        ProductIdentity ProductIdentity,
        ProductInfo ProductInfo)
        : ProductEvent(
            Version, 
            ProductIdentity,
            ProductStatus.Active);
}
