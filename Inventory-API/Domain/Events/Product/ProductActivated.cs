using Domain.Entities.Product;
using Domain.Events.Product.Enum;
using InventoryApi.Model.Events.Product;

namespace Domain.Events.Product
{
    public record ProductActivated(
        int Version,
        ProductIdentity ProductIdentity)
        : ProductEvent(
            Version, 
            ProductIdentity,
            ProductStatus.Active);
}
