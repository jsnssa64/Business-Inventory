using System.Security.Claims;
using Domain.ValueObjects.User;
using Microsoft.AspNetCore.Http;
using Services.Service.User.Input;

namespace Services.Service.User
{
    public interface IUserService
    {
        Task AssignUserToRole(UserIdInput userIdInput, RoleNameModel roleIdModel);
        Task ChangePassword(UserIdInput userIdInput, PasswordModel passwordModel);
        Task ForgottenPasswordByEmail(UserEmailModel userEmailModel);
        Task ForgottenPasswordByUsername(UserIdInput userIdInput);
        Task<UserDetails> GetUserDetails(UserIdInput userIdInput);
        Task LoginUser(HttpResponse httpResponse, UserLoginModel userLoginModel);
        void LogoutUser(HttpResponse httpResponse);
        Task<IEnumerable<Claim>> GenerateLogin(HttpResponse httpResponse, UserIdInput userIdInput, Action<UserWithPassword>? validate = null);
        Task RegisterDefaultUser(HttpResponse httpResponse, UserIdInput userIdentifierModel, UserRegistrationModel userRegistrationModel);
        Task RegisterUser(HttpResponse httpResponse, UserIdInput userIdInput, UserRegistrationModel userRegistrationModel);
        Task ResetPassword(string token, PasswordModel passwordModel);
        Task SetUserStatus(UserIdInput userIdInput, StatusModel statusModel);
        Task UpdateUser(UserIdInput userIdInput, UserDetailsModel userDetailsModel);
        Task UserConfirmation(string token);
        Task<User> GetUser(UserIdInput userIdInput);
        Task<IEnumerable<User>> GetUsers();
    }
}
