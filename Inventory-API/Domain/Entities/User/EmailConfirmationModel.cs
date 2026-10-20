using MediatR;

namespace Domain.Entities.User
{
    public class EmailConfirmationModel(string username, string email, string token) : INotification
    {
        public string Username { get; set; } = username;
        public string Email { get; set; } = email;
        public string Token { get; set; } = token;
    }
}


