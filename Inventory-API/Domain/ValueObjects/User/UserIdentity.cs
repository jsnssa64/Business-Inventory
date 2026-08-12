namespace Domain.ValueObjects.User
{
    public record UserIdentity(UserId Id, string Username, string Email);
}
