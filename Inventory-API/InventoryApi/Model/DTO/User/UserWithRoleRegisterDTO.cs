namespace InventoryApi.Model.DTO.User
{
    public class UserWithRoleRegisterDTO: UserRegisterDTO
    {
        public required string RolePublicId { get; set; }
    }
}
