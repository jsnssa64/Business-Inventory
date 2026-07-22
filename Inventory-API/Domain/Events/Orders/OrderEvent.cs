using Domain.Entities.Order;
using Domain.Events.Orders.Enum;

namespace InventoryApi.Model.Events.Product
{
    public record OrderEvent(OrderIdentity OrderIdentity, OrderStatus OrderStatus, int Version) : DomainEvent("Order", Version);
}
