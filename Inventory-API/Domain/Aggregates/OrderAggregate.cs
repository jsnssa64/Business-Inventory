using Domain.Entities.Order;
using Domain.Events.Orders;
using Domain.Events.Orders.Enum;
using Domain.ValueObjects.Order;

namespace Domain.Aggregates
{
    public class OrderAggregate : Aggregate
    {
        public Order CurrentOrder { get; set; }

        public void CreateOrder(int eventVersion, OrderIdentity orderIdentity)
        {
            var orderEvent = new OrderCreated(eventVersion, orderIdentity);

            AddEvent(orderEvent);

            Apply(orderEvent);

            Commit();
        }

        public void OrderStatusChanged(OrderIdentity orderIdentity, OrderStatus orderStatus, int eventVersion)
        {
            var orderEvent = new OrderStatusChanged(eventVersion, orderIdentity, orderStatus);

            AddEvent(orderEvent);

            Apply(orderStatus);

            Commit();

        }

        public void Apply(OrderCreated orderCreated)
        {
            CurrentOrder = new Order()
            {
                Id = orderCreated.OrderIdentity,
                Status = orderCreated.OrderStatus
            };
        }

        public void Apply(OrderStatus orderStatus)
        {
            CurrentOrder = CurrentOrder with { Status = orderStatus };
        }
    }
}
