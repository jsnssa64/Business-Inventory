using Domain.User;
using InventoryApi.Repository.Data.Role;

namespace InventoryApi.Repository.RoleRepo
{
    public interface IRoleRepository
    {
        Task<Role> CreateRole(RoleNameModel roleModel);
        Task<string> GetDefaultRole();
        Task<IEnumerable<Role>> GetRoles();
        Task<bool> IsValidRole(string rolePublicId);
    }
}
