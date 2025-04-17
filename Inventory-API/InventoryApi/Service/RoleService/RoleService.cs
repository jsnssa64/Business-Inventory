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
        public async Task<Role> CreateRole(string roleName)
        {
            if (roleName.IsNullOrEmpty())
            {
                throw new Exception("RolePublicId is required");
            }

            return await _roleRepository.CreateRole(new RoleNameModel() { RoleName = roleName });
        }
    }
}
