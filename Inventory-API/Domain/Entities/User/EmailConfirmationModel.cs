using MediatR;

namespace InventoryApi.Service.UserService
{
    public class EmailConfirmationModel : INotification
    {
        public string Username { get; internal set; }
        public string Email { get; internal set; }
        public string Token { get; internal set; }
    }
}


