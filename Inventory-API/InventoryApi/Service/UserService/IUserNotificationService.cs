using MediatR;

namespace InventoryApi.Service.UserService
{
    public class IUserNotificationService : INotificationHandler<UserCreatedModel>
    {
        public Task Handle(UserCreatedModel userCreatedModel, CancellationToken cancellationToken)
        {
            // Handle the notification logic here
            // For example, you might log the notification or send it to a message queue
            return Task.CompletedTask;
        }
    }

    public class UserCreatedModel : INotification
    {
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public UserCreatedModel(string message)
        {
            Message = message;
            Timestamp = DateTime.UtcNow;
        }
    }
}
