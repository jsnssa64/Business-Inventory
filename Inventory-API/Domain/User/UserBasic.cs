namespace Domain.User
{
    public class User
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public string? RoleName { get; set; }
        public void Map(dynamic user)
        {
            Username = user.Username;
            Email = user.Email;
            PasswordHash = user.PasswordHash;
            RoleName = user.RoleName;
        }
    }
}
