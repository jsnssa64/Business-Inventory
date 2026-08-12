using Domain.Events.Product.Enum;
using Domain.ValueObjects.Product;
using InventoryApi.Model.Events.Product;

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
