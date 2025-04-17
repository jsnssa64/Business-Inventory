using Domain.User;

namespace InventoryApi.Service.RoleService
{
    public interface IRoleService
    {
        Task<Role> CreateRole(string RolePublicId);
    }
}
