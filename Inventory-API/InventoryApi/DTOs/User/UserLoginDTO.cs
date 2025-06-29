namespace InventoryApi.DTOs.User
{
    public class UserLoginDTO
    {
        public required string UserName { get; set; }
        public required string UserPassword { get; set; }
    }
}
