namespace Domain.ValueObjects.User
{
    public record UserIdentity(int UserId, string UserName);
    public class UserClaims
    {
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string RoleName { get; set; }
    }
}
