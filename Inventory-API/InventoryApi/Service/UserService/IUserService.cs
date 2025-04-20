using System.Security.Claims;
using Domain.User;
using InventoryApi.Repository.Data.Product;
using InventoryApi.Repository.Data.Role;
using InventoryApi.Repository.Data.User;

namespace InventoryApi.Service.UserService
{
    public interface IUserService
    {
        Task AssignUserToRole(UserIdentifierModel userIdentifierModel, RoleIdModel roleIdModel);
        Task ForgottenPasswordByEmail(UserEmailModel userEmailModel);
        Task ForgottenPasswordByUsername(UserIdentifierModel userIdentifierModel);
        Task<(User, UserDetails)> GetUserDetails(UserIdentifierModel userName);
        Task LoginUser(HttpResponse httpResponse, UserIdentifierModel userIdentifierModel, PasswordModel passwordModel);
        void LogoutUser(HttpResponse httpResponse);
        Task<IEnumerable<Claim>> RefreshLogin(HttpResponse httpResponse, UserIdentifierModel userIdentifierModel, Action<User>? validate = null);
        Task RegisterDefaultUser(HttpResponse httpResponse, UserIdentifierModel userIdentifierModel, UserRegistrationModel userRegistrationModel);
        Task RegisterUser(HttpResponse httpResponse, UserIdentifierModel userIdentifierModel, UserRegistrationModel userRegistrationModel);
        Task ResetPassword(UserIdentifierModel userIdentifierModel, PasswordModel passwordModel);
        Task SetUserStatus(UserIdentifierModel userIdentifierModel, StatusModel statusModel);
        Task UpdateUser(UserIdentifierModel userIdentifierModel, UserDetailsModel userDetailsModel);
    }
}