namespace InventoryApi.DTOs.User
{
    public class UserWithRoleRegisterDTO: UserRegisterDTO
    {
        public required string RoleName { get; set; }
    }
}
