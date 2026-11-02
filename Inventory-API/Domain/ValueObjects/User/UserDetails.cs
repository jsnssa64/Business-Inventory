using Domain.ValueObjects.User.Role;
using Domain.ValueObjects.User.Role.Extensions;

namespace Domain.ValueObjects.User
{
    public class UserDetails
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public UserAddress? UserAddress { get; set; }
        public string? Gender { get; set; }
        public DateOnly? DOB { get; set; }
        public string? ContactNumber { get; set; }
        public UserRoleLevel Role { get; set; }

        public void Map(dynamic usrdets)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(usrdets);

                FirstName = usrdets?.FirstName;
                LastName = usrdets?.LastName;

                Role = UserRolesExtensions.GetUserRole((string)usrdets.RoleName);

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
