using InventoryApi.Repository.Data.Role;

namespace InventoryApi.Service.RoleService
{
    public interface IRoleService
    {
        Task<RoleModel> CreateRole(CreateRoleModel createRoleModel);
        Task<RoleModel> GetDefaultRole();
        Task<IEnumerable<RoleModel>> GetRoles();
    }
}
