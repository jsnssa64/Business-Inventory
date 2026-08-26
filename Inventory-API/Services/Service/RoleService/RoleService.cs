using Services.DataModel.Role;
using Services.Repository.RoleRepo;
using Domain.ValueObjects.User;

namespace Services.Service.RoleService
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository) 
        {
            _roleRepository = roleRepository;
        }

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
