namespace InventoryApi.Model.DTO.User
{
    public class ResetPasswordDTO
    {
        public required string newPassword { get; set; }
        public required string token { get; set; }
    }
}
