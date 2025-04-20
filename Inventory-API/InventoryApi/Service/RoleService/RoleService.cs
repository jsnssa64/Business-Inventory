using Domain.User;
using InventoryApi.Repository.Data.Role;
using InventoryApi.Repository.RoleRepo;
using Microsoft.IdentityModel.Tokens;

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
    }
}
