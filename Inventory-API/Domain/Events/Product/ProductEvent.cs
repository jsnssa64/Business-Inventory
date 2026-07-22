using Domain.Entities.Product;
using Domain.Events.Product.Enum;

namespace InventoryApi.Model.Events.Product
{
    public record ProductEvent(int Version, ProductIdentity ProductIdentity, ProductStatus ProductStatus) : DomainEvent("Product", Version);
}
