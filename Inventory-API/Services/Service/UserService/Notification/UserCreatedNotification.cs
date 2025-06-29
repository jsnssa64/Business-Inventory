using InventoryApi.Service.NotificationService;
using MassTransit;

namespace InventoryApi.Service.UserService.Notification
{
    public class UserCreatedNotification : EventNotificationService<UserCreated>
    {
        public UserCreatedNotification(IPublishEndpoint publishEndpoint) : base(publishEndpoint)
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


