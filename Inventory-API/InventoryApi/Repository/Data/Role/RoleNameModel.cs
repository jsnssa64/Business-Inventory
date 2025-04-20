namespace InventoryApi.Repository.Data.Role
{
    public class CreateRoleModel
    {
        public required string RoleName { get; set; }
        public required bool IsDefault { get; set; } = false;
    }

    public class RoleModel
    {
        public required Guid PublicRoleId { get; set; }
        public required string RoleName { get; set; }
        public required bool IsDefault { get; set; } = false;
    }
}
