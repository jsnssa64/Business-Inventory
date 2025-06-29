using Domain.Events.Orders.Enum;

namespace Domain.Entities.Order
{
    public readonly record struct Order(OrderIdentity orderIdentity, OrderStatus orderStatus);
}
