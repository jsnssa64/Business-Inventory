using System.Security.Claims;
using Domain.User;
using InventoryApi.Repository;
using InventoryApi.Repository.Data;
using InventoryApi.Repository.Data.Product;
using InventoryApi.Repository.Data.User;
using InventoryApi.Repository.RoleRepo;
using InventoryApi.Repository.UserRepo.Enum;
using InventoryApi.Service.SecurityService;
using InventoryApi.Service.SecurityService.Models;

namespace InventoryApi.Service.UserService
{
    public class UserService : IUserService
    {
        private IUserRepository _userRepository;
        private IRoleRepository _roleRepository;
        private ISecurityService _securityService;

        public UserService(IUserRepository userRepository, IRoleRepository roleRepository, ISecurityService securityService) {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _securityService = securityService;
        }

        public async Task RegisterUser(HttpResponse httpResponse, UserIdentifierModel userIdentifierModel, UserRegistrationModel userRegistrationModel)
        {
            try
            {
                if(string.IsNullOrEmpty(userRegistrationModel.RolePublicId))
                    throw new Exception("RolePublicId is required");

                if (!await _roleRepository.IsValidRole(userRegistrationModel.RolePublicId))
                    throw new Exception("Invalid Role");

                await this.CreateUser(httpResponse, userIdentifierModel, userRegistrationModel);
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
                userRegistrationModel.RolePublicId = await _roleRepository.GetDefaultRole();

                await this.CreateUser(httpResponse, userIdentifierModel, userRegistrationModel);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to register new User: {ex.Message}");
            }
        }

        private async Task CreateUser(HttpResponse httpResponse, UserIdentifierModel userIdentifierModel, UserRegistrationModel userRegistrationModel)
        {
            try
            {
                userRegistrationModel.Password = _securityService.EncryptPassword(userRegistrationModel.Password, SecurityLevel.Mid);

                await _userRepository.CreateUser(userIdentifierModel, userRegistrationModel);

                var passwordModel = new PasswordModel() { 
                    Password = userRegistrationModel.Password 
                };

                await LoginUser(httpResponse, userIdentifierModel, passwordModel);
            }
            catch(Exception ex)
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

        public UserClaims MapClaimsToUser(List<Claim> claims)
        {
            return new UserClaims()
            {
                Username =  claims.FirstOrDefault(c => c.Type == nameof(ClaimTypes.Name))?.Value ??  throw new Exception("Unable to generate Username claim"),
                Email =     claims.FirstOrDefault(c => c.Type == nameof(ClaimTypes.Email))?.Value ?? throw new Exception("Unable to generate Email claim"),
                RoleName =  claims.FirstOrDefault(c => c.Type == nameof(ClaimTypes.Role))?.Value ??  throw new Exception("Unable to generate Role claim")
            };
        }

        public IEnumerable<Claim> MapUserToClaims(User user)
        {
            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var claims = new List<Claim>()
            {
                new(ClaimTypes.Email,user.Email    ??  throw new Exception("Unable to generate Email claim")),     // Ensure non-null value
                new(ClaimTypes.Name, user.Username ??  throw new Exception("Unable to generate Username claim")),  // Ensure non-null value
                new(ClaimTypes.Role, user.RoleName ??  throw new Exception("Unable to generate Role claim"))       // Ensure non-null value
            };
            return claims;
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
            var user = await _userRepository.GetUser(userIdentifierModel, UserType.role) ?? throw new Exception("User not found");

            if (validate is not null)
            {
                validate(user);
            }

            var claims = this.MapUserToClaims(user);

            var cookieExpiry = DateTimeOffset.UtcNow.AddDays(1);

            var tokens = _securityService.GenerateLoginJWT(claims, claims);

            _securityService.SetCookieForLogin(httpResponse, tokens.AccessToken, tokens.RefreshToken, cookieExpiry);

            return claims;
        }

        public async Task<(User, UserDetails)> GetUserDetails(UserIdentifierModel userName)
        {
            var userDetails = await _userRepository.GetUserDetails(userName);

            return userDetails;
        }

        public void LogoutUser(HttpResponse httpResponse)
        {
            _securityService.SetCookieForLogout(httpResponse);
        }

        public async Task AssignUserToRole(UserIdentifierModel userIdentifierModel, RoleIdentifierModel roleIdentifierModel)
        {
            try
            {
                await _userRepository.AssignRoleToUser(userIdentifierModel, roleIdentifierModel);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to assign user to role: {ex.Message}");
            }
        }
    }
}
