using Domain.Entities.Order;
using Domain.Events.Orders.Enum;
using InventoryApi.Model.Events.Product;

namespace Domain.Events.Orders
{
    public record OrderCreated(int Version, OrderIdentity Orderidentity) : OrderEvent(Orderidentity, OrderStatus.Pending, Version);
} 
