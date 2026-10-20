using Domain.Service.UserService;
using MassTransit;
using Services.Service.NotificationService;

namespace Services.Service.UserService.Notification
{
    public class UserCreatedNotificationHandler : EventNotificationHandler<UserCreatedNotification>
    {
        public UserCreatedNotificationHandler(IPublishEndpoint publishEndpoint) : base(publishEndpoint)
        {
        }

        public override Task Handle(UserCreatedNotification notification, CancellationToken cancellationToken)
        {
            // Handle the user created notification  
            Console.WriteLine($"User Created: {notification.userIdentity.Id} at {notification.userIdentity.Username}");
            return Publish(notification, cancellationToken);
        }
    }
}


