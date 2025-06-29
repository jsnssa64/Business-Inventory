using Domain.Entities.User;

namespace InventoryApi.Service.UserService
{
    public record UserCreated(int Version, UserIdentity UserIdentity) : UserEvent(Version, UserIdentity);
}


