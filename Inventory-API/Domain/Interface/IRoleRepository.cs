

namespace Domain.Interface
{
    public interface IRoleRepository
    {
        [Obsolete("This method will be implemented in a future version")]
        Task CreateRole(Role role);

        [Obsolete("This method will be implemented in a future version")]
        Task<Role> GetDefaultRole();

        [Obsolete("This method will be implemented in a future version")]
        Task<IEnumerable<Role>> GetRoles();

        [Obsolete("This method will be implemented in a future version")]
        Task<bool> IsValidRole(RoleName roleIdModel);
    }
}
