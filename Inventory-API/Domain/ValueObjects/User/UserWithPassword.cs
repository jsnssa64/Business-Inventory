using Domain.Entities.User;

namespace Domain.ValueObjects.User
{
    public class UserWithPassword: User
    {
        public string PasswordHash { get; set; } = string.Empty;

        public void MapWithPassword(dynamic? user)
        {
            try
            {
                Map(user);
                PasswordHash = user?.PasswordHash!;
            }
            catch
            {
                throw new Exception("Unable to convert db user to domain user");
            }
        }
    }
}
