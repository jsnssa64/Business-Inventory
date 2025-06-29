using InventoryApi.Service.NotificationService;
using MassTransit;

namespace InventoryApi.Service.UserService.Notification
{
    public class EmailNotification : EventNotificationService<UserCreated>
    {
        public EmailNotification(IPublishEndpoint publishEndpoint) : base(publishEndpoint)
        {
        }

        public override Task Handle(UserCreated notification, CancellationToken cancellationToken)
        {
            // Handle the user created notification  
            Console.WriteLine($"User Created: {notification.Id} at {notification.Username}");
            return Publish(notification, cancellationToken);
        }
    }
}


