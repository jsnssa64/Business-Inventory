using System.Data;
using Domain.Entities.User;
using Services.DataModel.Role;
using Services.DataModel.User;
namespace Services.Repository.UserRepo
{
    public interface IUserRepository
    {
        Task<UserIdModel> CreateUser(UserIdentifierModel userIdentifier, UserRegistrationModel userRegisterModel);
        Task<UserIdModel> CreateUser(UserIdentifierModel userIdentifier, UserRegistrationModel userRegisterModel, IDbConnection dbConnection, IDbTransaction? dbTransaction = null);
        Task UpdateUserDetails(UserIdentifierModel userIdentifier, UserDetailsModel useDetailsModel);
        Task ActivateUser(UserIdentifierModel userIdentifier);
        Task ActivateUser(UserIdentifierModel userIdentifier, IDbConnection dbConnection, IDbTransaction? dbTransaction = null);
        Task AssignRoleToUser(UserIdentifierModel userIdentifier, RoleNameModel roleNameModel);
        Task AssignRoleToUser(UserIdentifierModel userIdentifier, RoleNameModel roleNameModel, IDbConnection dbConnection, IDbTransaction? dbTransaction = null);
        Task ResetPassword(UserIdentifierModel userIdentifier, PasswordModel passwordModel);
        Task SetUserStatus(UserIdentifierModel userIdentifier, StatusModel statusModel);
        Task<IEnumerable<User?>> GetAllUsers();
        Task<UserWithPassword?> GetUser(UserIdentifierModel userIdentifierModel);
        Task<User?> GetUserByEmail(UserEmailModel userEmailModel);
        Task<bool> IsValidUserByEmail(UserEmailModel userEmailModel);
        Task<bool> IsValidUserByUsername(UserIdentifierModel userIdentifierModel);
        Task<UserDetails> GetUserDetails(UserIdentifierModel userIdentifierModel);
    }
}
