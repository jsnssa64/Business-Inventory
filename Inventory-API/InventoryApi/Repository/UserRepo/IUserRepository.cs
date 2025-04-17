using Domain.User;
using InventoryApi.Repository.Data.Product;
using InventoryApi.Repository.Data.User;
using InventoryApi.Repository.UserRepo.Enum;
namespace InventoryApi.Repository
{
    public interface IUserRepository
    {
        Task AssignRoleToUser(UserIdentifierModel userIdentifier, RoleIdentifierModel roleIdentifierModel);
        Task CreateNewPassword(UserIdentifierModel userIdentifier, PasswordModel passwordModel);
        Task<int> CreateUser(UserIdentifierModel userIdentifier, UserRegistrationModel userRegisterModel);
        Task<User> GetUser(UserIdentifierModel userIdentifierModel, UserType userType);
        Task<(User, UserDetails)> GetUserDetails(UserIdentifierModel userIdentifierModel);
        Task SetUserStatus(UserIdentifierModel userIdentifier, StatusModel statusModel);
        Task UpdateUserDetails(UserIdentifierModel userIdentifier, UserDetailsModel useDetailsModel);
    }
}