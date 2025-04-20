using Domain.User;
using InventoryApi.Repository.Data.Product;
using InventoryApi.Repository.Data.Role;
using InventoryApi.Repository.Data.User;
using InventoryApi.Repository.UserRepo.Enum;
namespace InventoryApi.Repository
{
    public interface IUserRepository
    {
        Task ActivateUser(UserIdentifierModel userIdentifier);
        Task AssignRoleToUser(UserIdentifierModel userIdentifier, RoleIdModel roleIdModel);
        Task SetUserStatus(UserIdentifierModel userIdentifier, StatusModel statusModel);
        Task<User> GetUser(UserIdentifierModel userIdentifierModel);
        Task<(User, UserDetails)> GetUserDetails(UserIdentifierModel userIdentifierModel);
        Task UpdateUserDetails(UserIdentifierModel userIdentifier, UserDetailsModel useDetailsModel);
        Task<UserIdModel> CreateUser(UserIdentifierModel userIdentifier, UserRegistrationModel userRegisterModel);
        Task<bool> IsValidUserByUsername(UserIdentifierModel userIdentifierModel);
        Task<bool> IsValidUserByEmail(UserEmailModel userEmailModel);
        Task ResetPassword(UserIdentifierModel userIdentifier, PasswordModel passwordModel);
    }
}