using MassTransit;
using NotificationService.Consumer.Notification.Message;

namespace NotificationService.Consumer.Notification
{
    public class BatchMessageConsumer(ILogger<BatchMessageConsumer> logger): IConsumer<Batch<NotificationMessage>>
    {
        public async Task Consume(ConsumeContext<Batch<NotificationMessage>> context)
        {
            for (int i = 0; i < context.Message.Length; i++)
            {
                var message = context.Message[i].Message;
                logger.LogInformation("New message received: ");
            }
        }
    }
}
