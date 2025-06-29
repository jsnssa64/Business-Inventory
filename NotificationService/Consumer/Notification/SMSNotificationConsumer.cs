using MassTransit;
using NotificationService.Consumer.Notification.Message;

namespace NotificationService.Consumer.Notifications
{
    public class SMSNotificationConsumer : IConsumer<NotificationMessage>
    {
        readonly ILogger _logger;

        public SMSNotificationConsumer(ILogger<SMSNotificationConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<NotificationMessage> context)
        {
            

            _logger.LogInformation(
                "SMS - New Notification Message: {message}",
                context.Message
            );
        }
    }
}
