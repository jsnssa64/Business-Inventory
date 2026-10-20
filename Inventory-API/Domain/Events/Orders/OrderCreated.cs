using Domain.Events.Orders.Enum;
using Domain.ValueObjects.Order;
using InventoryApi.Model.Events.Product;

namespace Domain.Events.Orders
{
    public record OrderCreated(int Version, OrderIdentity Orderidentity) : OrderEvent(Orderidentity, OrderStatus.Pending, Version);
} 
