using Domain.Events.User;
using MassTransit;
using Services.Service.Notification;

namespace Services.Service.User.Notification
{
    public class EmailNotificationHandler : EventNotificationHandler<UserCreatedNotification>
    {
        public EmailNotificationHandler(IPublishEndpoint publishEndpoint) : base(publishEndpoint)
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


