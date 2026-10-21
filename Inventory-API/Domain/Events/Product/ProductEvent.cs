using Domain.Events.Product.Enum;
using Domain.ValueObjects.Product;

namespace Domain.Events.Product
{
    public record ProductEvent(int Version, ProductIdentity ProductIdentity, ProductStatus ProductStatus) : DomainEvent("Product", Version);
}
