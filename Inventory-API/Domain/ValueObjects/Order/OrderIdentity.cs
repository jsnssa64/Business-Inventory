using Domain.ValueObjects.Product;

namespace Domain.ValueObjects.Order
{
    public readonly record struct OrderIdentity(int id, Guid UserId, ProductIdentity productIdentity);
}
