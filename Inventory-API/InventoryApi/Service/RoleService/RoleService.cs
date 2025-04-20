using InventoryApi.Repository.Data.Role;
using InventoryApi.Repository.RoleRepo;

namespace InventoryApi.Service.RoleService
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository) 
        {
            _roleRepository = roleRepository;
        }
        public async Task<RoleModel> CreateRole(CreateRoleModel createRoleModel)
        {
            return await _roleRepository.CreateRole(createRoleModel);
        }

        public async Task<IEnumerable<RoleModel>> GetRoles()
        {
            return await _roleRepository.GetRoles();
        }

        public async Task<RoleModel> GetDefaultRole()
        {
            return await _roleRepository.GetDefaultRole();
        }
    }
}
