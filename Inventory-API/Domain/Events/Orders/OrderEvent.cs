using Domain.Events.Orders.Enum;
using Domain.ValueObjects.Order;

namespace InventoryApi.Model.Events.Product
{
    public record OrderEvent(OrderIdentity OrderIdentity, OrderStatus OrderStatus, int Version) : DomainEvent("Order", Version);
}
