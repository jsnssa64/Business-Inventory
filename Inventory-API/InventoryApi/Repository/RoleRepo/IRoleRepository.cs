using Domain.User;
using InventoryApi.Repository.Data.Role;
using InventoryApi.Repository.Data.User;

namespace InventoryApi.Repository.RoleRepo
{
    public interface IRoleRepository
    {
        Task<RoleModel> CreateRole(CreateRoleModel createRole);
        Task<RoleModel> GetDefaultRole();
        Task<IEnumerable<RoleModel>> GetRoles();
        Task<bool> IsValidRole(RoleIdentifierModel roleIdentifier);
    }
}
