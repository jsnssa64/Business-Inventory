namespace InventoryApi.Model.DTO.User
{
    public class UsersRoleDTO
    {
        public required string UserName { get; set; }
        public required Guid RoleId { get; set; }
    }
}
