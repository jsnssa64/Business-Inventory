namespace Domain.Entities.User
{
    public class User
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserRole Role { get; set; } = new UserRole();

        public void Map(dynamic? user)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(user);

                Username = user?.Username!;
                Email = user?.Email!;
                Role = new UserRole()
                {
                    Rolename = user?.RoleName!
                };
            }
            catch
            {
                throw new Exception("Unable to convert db user to domain user");
            }
        }
    }
}
