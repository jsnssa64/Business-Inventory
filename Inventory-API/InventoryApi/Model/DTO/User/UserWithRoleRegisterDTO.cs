namespace InventoryApi.Model.DTO.User
{
    public class UserWithRoleRegisterDTO: UserRegisterDTO
    {
        public required Guid RoleId { get; set; }
    }
}
