using MediatR;

namespace Domain.Service.UserService
{
    public class EmailConfirmationModel : INotification
    {
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string Token { get; set; }
    }
}


