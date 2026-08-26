using Domain.Events.Product.Enum;
using Domain.ValueObjects.Product;
using InventoryApi.Model.Events.Product;

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
