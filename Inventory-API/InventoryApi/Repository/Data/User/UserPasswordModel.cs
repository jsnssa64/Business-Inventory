namespace InventoryApi.Repository.Data.User
{
    public class PasswordModel
    {
        public required string OldPassword { get; set; }
        public required string NewPassword { get; set; }
    }
}
