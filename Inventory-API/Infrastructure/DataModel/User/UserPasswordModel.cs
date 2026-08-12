namespace Services.DataModel.User
{
    public class PasswordModel
    {
        public required string OldPassword { get; set; }
        public required string NewPassword { get; set; }
    }
}
