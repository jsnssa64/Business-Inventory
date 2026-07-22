namespace Domain.Events.Orders.Enum
{
    public enum OrderStatus
    {
        Pending,    // Order has been placed but not yet processed
        Processing, // Order is being processed
        Shipped,    // Order has been shipped
        Delivered,  // Order has been delivered to the customer
        Cancelled,  // Order has been cancelled
        Refunded    // Order has been refunded
    }
}
