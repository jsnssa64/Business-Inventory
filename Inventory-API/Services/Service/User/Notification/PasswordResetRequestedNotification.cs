using MediatR;

namespace Services.Service.User.Notification
{
    public class PasswordResetRequestedNotification : INotification
    {
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string Token { get; set; }
    }
}
