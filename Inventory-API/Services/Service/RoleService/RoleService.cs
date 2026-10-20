using Services.Repository.RoleRepo;
using Services.DataModel.Role;

namespace Services.Service.RoleService
{
    public class RoleService(IRoleRepository roleRepository) : IRoleService
    {
        private readonly IRoleRepository _roleRepository = roleRepository;

        [Obsolete]
        public async Task<RoleModel> CreateRole(CreateRoleModel createRoleModel)
        {
            return await _roleRepository.CreateRole(createRoleModel);
        }

        public IEnumerable<RoleModel> GetRoles()
        {
            var roles = Roles.AllRoles.Select(roleName =>
            {
                return new RoleModel
                {
                    IsDefault = Roles.DefaultRole == roleName,
                    RoleName = roleName.ToString()
                };
            });

            return roles;
        }

        public RoleModel GetDefaultRole()
        {
            return new RoleModel() { 
                IsDefault = true, 
                RoleName = Roles.DefaultRole.ToString()
            };
        }
    }
}
