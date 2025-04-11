using Domain.User;

namespace InventoryApi.Service.UserService
{
    public interface IUserService
    {
        void AssignUserToRole(int userId, Role role);
        void RegisterUser(User user);
    }
}