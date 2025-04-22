namespace Domain.User
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
                this.FirstName = usrdets?.FirstName;
                this.LastName = usrdets?.LastName;

                this.Role = new UserRole()
                {
                    Rolename = usrdets?.RoleName
                };

                this.userAddress = new UserAddress().Map(usrdets);

                this.ContactNumber = usrdets?.ContactNumber;
                this.DOB = usrdets?.DOB;
                this.Gender = usrdets?.Gender;
            }
            catch
            {
                throw new Exception("unable to convert to userdetails");
            }
        }
    }
}
