using Domain.Entities.Product;

namespace Domain.Entities.Order
{
    public readonly record struct OrderIdentity(int id, Guid UserId, ProductIdentity productIdentity);
}
