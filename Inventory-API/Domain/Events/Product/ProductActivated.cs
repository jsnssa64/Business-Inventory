using Domain.Entities.Product;
using Domain.Events.Product.Enum;

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
