using Domain.Entities.Order;
using Domain.Events.Orders.Enum;
using Domain.ValueObjects.Order;

namespace Domain.Events.Orders
{
    public record OrderStatusChanged(int Version, OrderIdentity Orderidentity, OrderStatus OrderStatus) : OrderEvent(Orderidentity, OrderStatus, Version);
}
