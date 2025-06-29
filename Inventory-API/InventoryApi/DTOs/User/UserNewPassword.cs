namespace InventoryApi.DTOs.User
{
    public class UserNewPassword
    {
        public required string NewPassword { get; set; }
        public required string OldPassword { get; set; }
    }
}
