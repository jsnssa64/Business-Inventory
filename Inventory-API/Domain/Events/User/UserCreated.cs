using Domain.Entities.User;

namespace Domain.Events.User
{
    public record UserCreated(int Version, UserIdentity UserIdentity) : UserEvent(Version, UserIdentity);
}


