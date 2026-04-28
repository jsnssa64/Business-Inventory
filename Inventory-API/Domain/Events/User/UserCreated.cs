using Domain.Entities.User;
using MediatR;

namespace Domain.Service.UserService
{
    public class UserCreatedNotification: INotification
    {
        public int Version { get; set; } = 0;
        public required UserIdentity userIdentity { get; set; }
    }

}


