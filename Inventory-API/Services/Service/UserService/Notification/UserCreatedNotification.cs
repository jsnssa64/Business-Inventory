using Domain.Events.User;
using MassTransit;
using Services.Service.NotificationService;

namespace Services.Service.UserService.Notification
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


