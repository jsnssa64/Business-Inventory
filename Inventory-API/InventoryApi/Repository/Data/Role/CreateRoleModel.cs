namespace InventoryApi.Repository.Data.Role
{
    [Obsolete]
    public class CreateRoleModel
    {
        public required string RoleName { get; set; }
        public required bool IsDefault { get; set; } = false;
    }
}
