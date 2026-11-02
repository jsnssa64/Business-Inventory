namespace Domain.Entities.User
{
    public class UserLogin
    {
        public required string Username { get; set; }
        public string? Email { get; set; }
        public required string Password { get; set; }


        //  TODO: Check UserName and Email are not null or empty before creating UserLogin object
        //  TODO: Check Password is not null or empty before creating UserLogin object
    }
}
