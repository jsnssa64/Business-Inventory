using System.Data;
using Domain.Entities.User;
using InventoryApi.Repository.UserRepo.Enum;
using Services.DataModel.Role;
using Services.DataModel.User;

namespace Services.Interface.User
{
    public interface IUserRepository
    {
        Task ActivateUser(IDbConnection dbConnection, UserIdentifierModel userIdentifier, IDbTransaction? dbTransaction = null);
        Task AssignRoleToUser(IDbConnection dbConnection, UserIdentifierModel userIdentifier, RoleNameModel roleIdModel, IDbTransaction? dbTransaction = null);
        Task SetUserStatus(UserIdentifierModel userIdentifier, StatusModel statusModel);
        Task<UserWithPassword> GetUser(UserIdentifierModel userIdentifierModel);
        Task<User> GetUserByEmail(UserEmailModel userEmailModel);
        Task<UserDetails> GetUserDetails(UserIdentifierModel userIdentifierModel);
        Task UpdateUserDetails(UserIdentifierModel userIdentifier, UserDetailsModel useDetailsModel);
        Task<UserIdModel> CreateUser(IDbConnection dbConnection, UserIdentifierModel userIdentifier, UserRegistrationModel userRegisterModel, IDbTransaction? dbTransaction = null);
        Task<bool> IsValidUserByUsername(UserIdentifierModel userIdentifierModel);
        Task<bool> IsValidUserByEmail(UserEmailModel userEmailModel);
        Task ResetPassword(UserIdentifierModel userIdentifier, PasswordModel passwordModel);
        Task<IEnumerable<User>> GetAllUsers();
    }
}