using Domain.ValueObjects.User.Role;

namespace Domain.ValueObjects.User
{
    public class UserDetails
    {
        public UserIdentity? UserIdentity { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public UserAddress? UserAddress { get; set; }
        public string? Gender { get; set; }
        public DateOnly? DOB { get; set; }
        public string? ContactNumber { get; set; }
        public UserRoleLevel Role { get; set; }
        public UserEncryptedPassword? Password { get; set; }

        public void Map(dynamic usrdets)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(usrdets);

                FirstName = usrdets?.FirstName;
                LastName = usrdets?.LastName;

                Role = UserRole.GetUserRole((string)usrdets.RoleName);

                UserAddress = new UserAddress();
                UserAddress.Map(usrdets);

                ContactNumber = usrdets?.ContactNumber;
                DOB = usrdets?.DOB;
                Gender = usrdets?.Gender;
            }
            catch
            {
                throw new Exception("unable to convert to userdetails");
            }
        }
    }
}
