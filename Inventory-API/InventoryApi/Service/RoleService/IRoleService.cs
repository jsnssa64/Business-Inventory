using InventoryApi.Repository.Data.Role;

namespace InventoryApi.Service.RoleService
{
    public interface IRoleService
    {
        [Obsolete]
        Task<RoleModel> CreateRole(CreateRoleModel createRoleModel);
        RoleModel GetDefaultRole();
        IEnumerable<RoleModel> GetRoles();
    }
}
