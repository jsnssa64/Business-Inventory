using Domain.Entities.Product;
using Domain.Events.Product.Enum;
using InventoryApi.Model.Events.Product;

namespace Domain.Events.Product
{
    public record ProductDeactivated(
        int Version,
        ProductIdentity ProductIdentity)
        : ProductEvent(
            Version, 
            ProductIdentity,
            ProductStatus.Inactive);
}
