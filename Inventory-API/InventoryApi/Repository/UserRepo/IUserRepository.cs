using Domain.User;
using InventoryApi.Repository.Data.Product;
using InventoryApi.Repository.Data.User;
using InventoryApi.Repository.UserRepo.Enum;
namespace InventoryApi.Repository
{
    public interface IUserRepository
    {
        Task ActivateUser(UserIdentifierModel userIdentifier);
        Task AssignRoleToUser(UserIdentifierModel userIdentifier, RoleIdentifierModel roleIdentifierModel);
        Task CreateNewPassword(UserIdentifierModel userIdentifier, PasswordModel passwordModel);
        Task<int> CreateUser(UserIdentifierModel userIdentifier, UserRegistrationModel userRegisterModel);
        Task SetUserStatus(UserIdentifierModel userIdentifier, StatusModel statusModel);
        Task<User> GetUser(UserIdentifierModel userIdentifierModel);
        Task<(User, UserDetails)> GetUserDetails(UserIdentifierModel userIdentifierModel);
        Task UpdateUserDetails(UserIdentifierModel userIdentifier, UserDetailsModel useDetailsModel);
    }
}