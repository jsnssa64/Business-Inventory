using Domain.Entities.Product;
using Domain.Events.Product.Enum;

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
