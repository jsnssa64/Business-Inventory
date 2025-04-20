using System.Security.Claims;
using Domain.User;
using InventoryApi.Repository;
using InventoryApi.Repository.Data.Product;
using InventoryApi.Repository.Data.Role;
using InventoryApi.Repository.Data.User;
using InventoryApi.Repository.RoleRepo;
using InventoryApi.Service.SecurityService;
using InventoryApi.Service.SecurityService.Models;
using InventoryApi.Service.UserService.Utility;

namespace InventoryApi.Service.UserService
{
    public class UserService : IUserService
    {
        private IUserRepository _userRepository;
        private IRoleRepository _roleRepository;
        private ISecurityService _securityService;
        private IUserUtility _userUtility;
        private IJWTUtility _jwtUtility;

        public UserService(IUserRepository userRepository, IRoleRepository roleRepository, ISecurityService securityService, IUserUtility userUtility, IJWTUtility jwtUtility) {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _securityService = securityService;
            _userUtility = userUtility;
            _jwtUtility = jwtUtility;
        }

        public async Task RegisterUser(HttpResponse httpResponse, UserIdentifierModel userIdentifierModel, UserRegistrationModel userRegistrationModel)
        {
            try
            {
                if (userRegistrationModel.PublicRoleId is null)
                    throw new Exception("Unable to register user: Role Id missing");

                var roleIdModel = new RoleIdModel()
                {
                    PublicRoleId = (Guid)userRegistrationModel.PublicRoleId
                };

                if (!await _roleRepository.IsValidRole(roleIdModel))
                    throw new Exception("Invalid Role");

                var userIdModel = await this.CreateUser(httpResponse, userIdentifierModel, userRegistrationModel);
                
                await this.AssignUserToRole(userIdentifierModel, roleIdModel);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to register new User: {ex.Message}");
            }
        }

        public async Task RegisterDefaultUser(HttpResponse httpResponse, UserIdentifierModel userIdentifierModel, UserRegistrationModel userRegistrationModel)
        {
            try
            {
                var roleModel = await _roleRepository.GetDefaultRole();
                userRegistrationModel.PublicRoleId = roleModel.PublicRoleId;

                await this.CreateUser(httpResponse, userIdentifierModel, userRegistrationModel);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to register new User: {ex.Message}");
            }
        }

        private async Task<UserIdModel> CreateUser(HttpResponse httpResponse, UserIdentifierModel userIdentifierModel, UserRegistrationModel userRegistrationModel)
        {
            try
            {
                userRegistrationModel.Password = _securityService.EncryptPassword(userRegistrationModel.Password, SecurityLevel.Mid);

                var userIdModel = await _userRepository.CreateUser(userIdentifierModel, userRegistrationModel);

                var passwordModel = new PasswordModel() { 
                    Password = userRegistrationModel.Password 
                };

                //  Temp - 
                await this.ActivateUser(userIdentifierModel);

                await LoginUser(httpResponse, userIdentifierModel, passwordModel);

                return userIdModel;
            }
            catch(Exception ex)
            {
                throw new Exception($"Failed to register new User: {ex.Message}");
            }
        }

        private async Task ActivateUser(UserIdentifierModel userIdentifierModel)
        {
            try
            {
                await _userRepository.ActivateUser(userIdentifierModel);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to register new User: {ex.Message}");
            }
        }

        public async Task UpdateUser(UserIdentifierModel userIdentifierModel, UserDetailsModel userDetailsModel)
        {
            try
            {
                await _userRepository.UpdateUserDetails(userIdentifierModel, userDetailsModel);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to register new User: {ex.Message}");
            }
        }

        public async Task SetUserStatus(UserIdentifierModel userIdentifierModel, StatusModel statusModel)
        {
            try
            {
                await _userRepository.SetUserStatus(userIdentifierModel, statusModel);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to register new User: {ex.Message}");
            }
        }

        public async Task UserConfirmation(string token)
        {
            var tokenClaims = await _jwtUtility.GetTokenClaims(token, KeyType.confirmation);

            var userIdentifier = new UserIdentifierModel()
            {
                Username = _userUtility.GetClaimForUser(tokenClaims.Claims, UserClaim.Username)
            };

            await this.ActivateUser(userIdentifier);
        }

        public async Task ResetPassword(string token, PasswordModel passwordModel)
        {
            var tokenClaims = await _jwtUtility.GetTokenClaims(token, KeyType.resetPassword);

            var userIdentifier = new UserIdentifierModel()
            {
                Username = _userUtility.GetClaimForUser(tokenClaims.Claims, UserClaim.Username)
            };

            await _userRepository.ResetPassword(userIdentifier, passwordModel);
        }

        public async Task LoginUser(HttpResponse httpResponse, UserIdentifierModel userIdentifierModel, PasswordModel passwordModel)
        {
            await this.RefreshLogin(httpResponse, userIdentifierModel, (usersRole) =>
            {
                if (!_securityService.VerifyPassword(passwordModel.Password, usersRole.PasswordHash ?? throw new Exception("Unable to verify password")))
                {
                    throw new Exception("Invalid password");
                }
            });
        }

        public async Task<IEnumerable<Claim>> RefreshLogin(HttpResponse httpResponse, UserIdentifierModel userIdentifierModel, Action<User>? validate = null)
        {
            var user = await _userRepository.GetUser(userIdentifierModel) ?? throw new Exception("User not found");

            if (validate is not null)
            {
                validate(user);
            }

            var claims = _userUtility.MapUserToClaims(user);

            var cookieExpiry = DateTimeOffset.UtcNow.AddDays(1);

            var tokens = _securityService.GenerateLoginJWT(claims, claims);

            _securityService.SetCookieForLogin(httpResponse, tokens.AccessToken, tokens.RefreshToken, cookieExpiry);

            return claims;
        }

        public async Task<(User, UserDetails)> GetUserDetails(UserIdentifierModel userName)
        {
            return await _userRepository.GetUserDetails(userName);
        }

        public async Task ForgottenPasswordByEmail(UserEmailModel userEmailModel)
        {
            if (!await _userRepository.IsValidUserByEmail(userEmailModel)) 
            {
                throw new Exception("Invalid Email");
            }

            //  Trigger Email - Password - user.Email
        }

        public async Task ForgottenPasswordByUsername(UserIdentifierModel userIdentifierModel)
        {
            if (!await _userRepository.IsValidUserByUsername(userIdentifierModel))
            {
                throw new Exception("Invalid Username");
            }

            var user = await _userRepository.GetUser(userIdentifierModel);

            //  Trigger Email - Password - user.Email
        }

        public async Task ChangePassword(UserIdentifierModel userIdentifierModel, PasswordModel passwordModel)
        {
            try
            {
                await _userRepository.ResetPassword(userIdentifierModel, passwordModel);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to Reset User password: {ex.Message}");
            }
        }

        public void LogoutUser(HttpResponse httpResponse)
        {
            try
            {
                _securityService.SetCookieForLogout(httpResponse);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to Set User Cookie: {ex.Message}");
            }
        }

        public async Task AssignUserToRole(UserIdentifierModel userIdentifierModel, RoleIdModel roleIdModel)
        {
            try
            {
                await _userRepository.AssignRoleToUser(userIdentifierModel, roleIdModel);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to assign user to role: {ex.Message}");
            }
        }
    }
}
