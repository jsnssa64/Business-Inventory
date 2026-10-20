namespace Domain.ValueObjects.User
{
    public class UserClaims
    {
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string RoleName { get; set; }
    }
}
