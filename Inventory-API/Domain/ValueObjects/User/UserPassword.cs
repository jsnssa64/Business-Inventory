namespace Domain.ValueObjects.User
{
    public sealed record UserEncryptedPassword(string PasswordHash);
    public sealed record UserPassword(string Password);
}
