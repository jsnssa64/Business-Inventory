namespace Services.Service.User.Input
{
    public record UserChangePasswordInput(string OldPassword, string NewPassword);
}
