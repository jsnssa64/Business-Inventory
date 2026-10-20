namespace Domain.ValueObjects.User
{
    public sealed record UserWithPassword(Domain.Entities.User.User User, string PasswordHash);
}
