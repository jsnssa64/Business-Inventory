using Domain.Events.Product.Enum;
using Domain.ValueObjects.Product;

namespace InventoryApi.Model.Events.Product
{
    public record ProductEvent(int Version, ProductIdentity ProductIdentity, ProductStatus ProductStatus) : DomainEvent("Product", Version);
}
