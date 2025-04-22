namespace InventoryApi.Model.DTO.User
{
    public class ResetPasswordDTO
    {
        public required string OldPassword { get; set; }
        public required string NewPassword { get; set; }
        public required string token { get; set; }
    }
}
