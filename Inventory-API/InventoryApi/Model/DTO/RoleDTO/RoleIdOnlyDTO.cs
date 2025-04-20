namespace InventoryApi.Model.DTO.RoleDTO
{
    public class CreateRoleDTO
    {
        public required string RoleName { get; set; }
        public bool SetAsDefault { get; set; } = false;

    }
}
