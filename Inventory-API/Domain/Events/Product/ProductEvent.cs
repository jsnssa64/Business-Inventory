using Domain.Entities.Product;
using Domain.Events.Product.Enum;

namespace Domain.Events.Product
{
    public record ProductEvent(int Version, ProductIdentity ProductIdentity, ProductStatus ProductStatus) : DomainEvent("Product", Version);
}
