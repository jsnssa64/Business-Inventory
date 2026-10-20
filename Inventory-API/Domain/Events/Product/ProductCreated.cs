using Domain.Entities.Product;
using Domain.Events.Product.Enum;

namespace Domain.Events.Product
{
    public record ProductCreated(
        int Version, 
        ProductIdentity ProductIdentity, 
        ProductStatus ProductStatus = ProductStatus.Active)
        : ProductEvent(
            Version, 
            ProductIdentity, 
            ProductStatus);
}
