namespace InventoryApi.Model.DTO.User
{
    public class UserLoginDTO
    {
        public required string UserName { get; set; }
        public required string UserEmail { get; set; }
        public required string UserPassword { get; set; }
    }
}
