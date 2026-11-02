using Domain.Events.Orders.Enum;
using Domain.ValueObjects.Order;

namespace Domain.Events.Orders.Core
{
    public record OrderEvent(OrderIdentity OrderIdentity, OrderStatus OrderStatus, int Version) : DomainEvent(EventType.Order, Version);
}
