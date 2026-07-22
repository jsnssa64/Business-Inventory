namespace Domain.Entities.User
{
    public class UserDetails
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public UserAddress? userAddress { get; set; }
        public string? Gender { get; set; }
        public DateOnly? DOB { get; set; }
        public string? ContactNumber { get; set; }
        public UserRole? Role { get; set; }

        public void Map(dynamic usrdets)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(usrdets);

                Username = usrdets?.Username;
                Email = usrdets?.Email;
                FirstName = usrdets?.FirstName;
                LastName = usrdets?.LastName;

                Role = new UserRole()
                {
                    Rolename = usrdets?.RoleName
                };

                userAddress = new UserAddress();
                userAddress.Map(usrdets);

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
