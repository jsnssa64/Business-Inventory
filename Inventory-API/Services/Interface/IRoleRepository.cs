using Services.DataModel.Role;

namespace Services.Interface
{
    public interface IRoleRepository
    {
        [Obsolete]
        Task<RoleModel> CreateRole(CreateRoleModel createRole);

        [Obsolete]
        Task<RoleModel> GetDefaultRole();

        [Obsolete]
        Task<IEnumerable<RoleModel>> GetRoles();

        [Obsolete]
        Task<bool> IsValidRole(RoleNameModel roleIdModel);
    }
}
