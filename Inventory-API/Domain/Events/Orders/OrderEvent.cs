using Domain.Entities.Order;
using Domain.Events.Orders.Enum;

namespace Domain.Events.Orders
{
    public record OrderEvent(OrderIdentity OrderIdentity, OrderStatus OrderStatus, int Version) : DomainEvent("Order", Version);
}
