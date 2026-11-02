using Domain.Interface;

namespace Services.Service.Role
{
    public class RoleService(IRoleRepository roleRepository) : IRoleService
    {
        private readonly IRoleRepository _roleRepository = roleRepository;

        //  TODO: When Role is moved to database but for now we will keep it in memory for simplicity.
    }
}
