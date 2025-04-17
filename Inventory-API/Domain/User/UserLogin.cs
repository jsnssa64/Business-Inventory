namespace Domain.User
{
    public class UserLogin
    {
        public required string Username { get; set; }
        public string? Email { get; set; }
        public required string Password { get; set; }
    }
}
