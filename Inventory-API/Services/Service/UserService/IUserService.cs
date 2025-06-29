using System.Security.Claims;
using Domain.Entities.User;
using Services.DataModel.Role;
using Services.DataModel.User;

namespace InventoryApi.Service.UserService
{
    public interface IUserService
    {
        Task AssignUserToRole(UserIdentifierModel userIdentifierModel, RoleNameModel roleIdModel);
        Task ChangePassword(UserIdentifierModel userIdentifierModel, PasswordModel passwordModel);
        Task ForgottenPasswordByEmail(UserEmailModel userEmailModel);
        Task ForgottenPasswordByUsername(UserIdentifierModel userIdentifierModel);
        Task<UserDetails> GetUserDetails(UserIdentifierModel userName);
        Task LoginUser(HttpResponse httpResponse, UserLoginModel userLoginModel);
        void LogoutUser(HttpResponse httpResponse);
        Task<IEnumerable<Claim>> GenerateLogin(HttpResponse httpResponse, UserIdentifierModel userIdentifierModel, Action<UserWithPassword>? validate = null);
        Task RegisterDefaultUser(HttpResponse httpResponse, UserIdentifierModel userIdentifierModel, UserRegistrationModel userRegistrationModel);
        Task RegisterUser(HttpResponse httpResponse, UserIdentifierModel userIdentifierModel, UserRegistrationModel userRegistrationModel);
        Task ResetPassword(string token, PasswordModel passwordModel);
        Task SetUserStatus(UserIdentifierModel userIdentifierModel, StatusModel statusModel);
        Task UpdateUser(UserIdentifierModel userIdentifierModel, UserDetailsModel userDetailsModel);
        Task UserConfirmation(string token);
        Task<User> GetUser(UserIdentifierModel userName);
        Task<IEnumerable<User>> GetUsers();
    }
}