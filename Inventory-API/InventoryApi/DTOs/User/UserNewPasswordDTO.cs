namespace InventoryApi.DTOs.User
{
    public class UserNewPasswordDTO
    {
        public required string NewPassword { get; set; }
        public required string OldPassword { get; set; }
    }
}
