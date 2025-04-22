using System.Data;
using Domain.User;
using InventoryApi.Repository.Data.Product;
using InventoryApi.Repository.Data.Role;
using InventoryApi.Repository.Data.User;
using InventoryApi.Repository.UserRepo.Enum;
namespace InventoryApi.Repository
{
    public interface IUserRepository
    {
        Task ActivateUser(IDbConnection dbConnection, UserIdentifierModel userIdentifier, IDbTransaction? dbTransaction = null);
        Task AssignRoleToUser(IDbConnection dbConnection, UserIdentifierModel userIdentifier, RoleNameModel roleIdModel, IDbTransaction? dbTransaction = null);
        Task SetUserStatus(UserIdentifierModel userIdentifier, StatusModel statusModel);
        Task<User> GetUser(UserIdentifierModel userIdentifierModel);
        Task<UserDetails> GetUserDetails(UserIdentifierModel userIdentifierModel);
        Task UpdateUserDetails(UserIdentifierModel userIdentifier, UserDetailsModel useDetailsModel);
        Task<UserIdModel> CreateUser(IDbConnection dbConnection, UserIdentifierModel userIdentifier, UserRegistrationModel userRegisterModel, IDbTransaction? dbTransaction = null);
        Task<bool> IsValidUserByUsername(UserIdentifierModel userIdentifierModel);
        Task<bool> IsValidUserByEmail(UserEmailModel userEmailModel);
        Task ResetPassword(UserIdentifierModel userIdentifier, PasswordModel passwordModel);
    }
}