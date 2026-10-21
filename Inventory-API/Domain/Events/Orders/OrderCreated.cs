using Domain.Entities.Order;
using Domain.Events.Orders.Enum;
using Domain.ValueObjects.Order;

namespace Domain.Events.Orders
{
    public record OrderCreated(int Version, OrderIdentity Orderidentity) : OrderEvent(Orderidentity, OrderStatus.Pending, Version);
} 
