namespace Domain.User
{
    public class UserRole
    {
        public int UserId { get; set; }
        public required Role Role { get; set; }
    }
}
