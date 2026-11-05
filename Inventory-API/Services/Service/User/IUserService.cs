using System.Security.Claims;
using Domain.ValueObjects.User;
using Microsoft.AspNetCore.Http;
using Services.Service.User.Dto;
using Services.Service.User.Input;

namespace Services.Service.User
{
    public interface IUserService
    {
        Task AssignUserToRole(UserIdInput userIdInput, UserRoleInput roleInput);
        Task ChangePassword(UserIdInput userIdInput, UserChangePasswordInput passwordChangeInput);
        Task ForgottenPasswordByEmail(UserEmailInput userEmailInput);
        Task ForgottenPasswordByUsername(UserIdInput userIdInput);
        Task<UserIdDto> GetUser(UserIdInput userIdInput);
        Task<IEnumerable<UserDetailsDto>> GetUsers();
        Task LogoutUser(HttpResponse httpResponse);
        Task RegisterDefaultUser(HttpResponse httpResponse, UserIdInput userId, UserDetailsInput userDetailsInput);
        Task RegisterUser(HttpResponse httpResponse, UserIdInput userId, UserDetailsInput userDetailsInput);
        Task ResetPassword(string token, UserPasswordInput password);
        Task SetUserStatus(UserIdInput userIdInput, UserStatus status);
        Task UpdateUser(UserIdInput userIdInput, UserDetailsInput userDetailsModel);
        Task UserConfirmation(string token);
    }
}
