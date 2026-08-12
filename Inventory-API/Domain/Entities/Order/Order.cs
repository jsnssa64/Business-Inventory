using Domain.Events.Orders.Enum;
using Domain.ValueObjects.Order;

namespace Domain.Entities.Order
{
    public readonly record struct Order(OrderIdentity orderIdentity, OrderStatus orderStatus);
}
