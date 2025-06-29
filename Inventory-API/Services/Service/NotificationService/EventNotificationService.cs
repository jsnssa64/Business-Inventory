using MassTransit;
using MediatR;

namespace InventoryApi.Service.NotificationService
{
    public abstract class EventNotificationService<T> : INotificationHandler<T> where T : INotification
    {
        private readonly IPublishEndpoint _publishEndpoint;

        protected EventNotificationService(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public abstract Task Handle(T notification, CancellationToken cancellationToken);

        public virtual async Task Publish(T notification, CancellationToken cancellationToken)
        {
            try
            {
                await _publishEndpoint.Publish(notification, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                // Handle the cancellation gracefully  
                Console.WriteLine("Notification handling was cancelled.");
                return;
            }
            catch (Exception ex)
            {
                // Log or handle any exceptions that occur during processing  
                Console.WriteLine($"Error handling notification: {ex.Message}");
                //  Database post and continue
                return;
            }
        }
    }
}


