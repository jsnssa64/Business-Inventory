namespace Domain.User
{
    public class User
    {
        public required int Id { get; set; }
        public string? UserName { get; set; }
        public string? EncryptedPassword { get; set; }
        public string? Email { get; set; }
        public Role? UserRole { get; set; } = null;
    }
}
