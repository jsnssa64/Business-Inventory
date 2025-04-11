using Domain.User;

namespace InventoryApi.Repository
{
    public interface IUserRepository
    {
        Task<bool> AssignRoleToUser(UserRole userRole);
        Task<User> RegisterUserDetails(User user, UserDetails userDetails);
        Task<User> RegisterUserLogin(User user);
    }
}