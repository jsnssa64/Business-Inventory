using Services.DataModel.Role;

namespace Services.Service.RoleService
{
    public interface IRoleService
    {
        [Obsolete]
        Task<RoleModel> CreateRole(CreateRoleModel createRoleModel);
        RoleModel GetDefaultRole();
        IEnumerable<RoleModel> GetRoles();
    }
}
