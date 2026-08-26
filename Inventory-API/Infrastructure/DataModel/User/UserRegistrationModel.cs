namespace Services.DataModel.User
{
    public class UserRegistrationModel
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string EncryptedPassword { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
    }

}
