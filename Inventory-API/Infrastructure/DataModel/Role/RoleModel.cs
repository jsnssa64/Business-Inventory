namespace Services.DataModel.Role
{
    public class RoleModel
    {
        public required string RoleName { get; set; }
        public required bool IsDefault { get; set; } = false;
    }
}
