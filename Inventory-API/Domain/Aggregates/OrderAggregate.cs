using Domain.Aggregates;
using Domain.Entities.Order;
using Domain.Events.Orders;
using Domain.Events.Orders.Enum;
using Domain.ValueObjects.Order;

namespace InventoryApi.Model.Action
{
    public class OrderAggregate : Aggregate
    {
        public Order currentOrder { get; set; }

        public void CreateOrder(int eventVersion, OrderIdentity orderIdentity)
        {
            var orderEvent = new OrderCreated(eventVersion, orderIdentity);

            AddEvent(orderEvent);

            Apply(orderEvent);

            commit();
        }

        public void OrderStatusChanged(OrderIdentity orderIdentity, OrderStatus orderStatus, int eventVersion)
        {
            var orderEvent = new OrderStatusChanged(eventVersion, orderIdentity, orderStatus);

            AddEvent(orderEvent);

            Apply(orderStatus);

            commit();

        }

        public void Apply(OrderCreated orderCreated)
        {
            currentOrder = new Order()
            {
                orderIdentity = orderCreated.OrderIdentity,
                orderStatus = orderCreated.OrderStatus
            };
        }

        public void Apply(OrderStatus orderStatus)
        {
            currentOrder = currentOrder with { orderStatus = orderStatus };
        }
    }
}
