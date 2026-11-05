using Domain.Events.Product.Core;
using Domain.Events.Product.Enum;
using Domain.ValueObjects.Product;

namespace Domain.Events.Product
{
    public record ProductCreated(
        int Version, 
        ProductIdentity ProductIdentity,
        Price Price,
        ProductStatus ProductStatus = ProductStatus.Active)
        : ProductEvent(
            Version, 
            ProductIdentity, 
            ProductStatus);
}
