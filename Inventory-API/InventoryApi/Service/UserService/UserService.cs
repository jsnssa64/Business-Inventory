using System.Data.Entity.Infrastructure;
using System.Data;
using System.Security.Claims;
using Domain.User;
using InventoryApi.Factory;
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
        private IDbConnectionFactory _dbConnectionFactory;

        public UserService(IUserRepository userRepository, 
            IRoleRepository roleRepository, 
            ISecurityService securityService, 
            IUserUtility userUtility, 
            IJWTUtility jwtUtility,
            IDbConnectionFactory dbConnectionFactory) {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _securityService = securityService;
            _userUtility = userUtility;
            _jwtUtility = jwtUtility;
            _dbConnectionFactory = dbConnectionFactory; 
        }

        public async Task RegisterUser(HttpResponse httpResponse, UserIdentifierModel userIdentifierModel, UserRegistrationModel userRegistrationModel)
        {
            try
            {
                var userIdModel = await this.CreateUser(httpResponse, userIdentifierModel, userRegistrationModel);
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
            using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());
            conn.Open();
            var transaction = conn.BeginTransaction();

            try
            {
                if (userRegistrationModel.PublicRoleId is null)
                    throw new Exception("Unable to register user: Role Id missing");

                userRegistrationModel.EncryptedPassword = _securityService.EncryptPassword(userRegistrationModel.Password, SecurityLevel.Mid);

                var userIdModel = await _userRepository.CreateUser(conn, userIdentifierModel, userRegistrationModel, transaction);

                var roleIdModel = new RoleIdModel
                {
                    PublicRoleId = (Guid)userRegistrationModel.PublicRoleId
                };

                await _userRepository.AssignRoleToUser(conn, userIdentifierModel, roleIdModel, transaction);
                
                //  Temp - 
                await _userRepository.ActivateUser(conn, userIdentifierModel, transaction);

                transaction.Commit();
                
                var passwordModel = new PasswordModel()
                {
                    Password = userRegistrationModel.Password
                };

                await LoginUser(httpResponse, userIdentifierModel, passwordModel);

                return userIdModel;
            }
            catch(Exception ex)
            {
                if(transaction?.Connection != null)
                    transaction.Rollback();
                throw new Exception($"Failed to register new User: {ex.Message}");
            }
        }

        private async Task ActivateUser(UserIdentifierModel userIdentifierModel)
        {
            try
            {
                using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());
                await _userRepository.ActivateUser(conn, userIdentifierModel);
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
            await this.GenerateLogin(httpResponse, userIdentifierModel, (usersRole) =>
            {
                if (!_securityService.VerifyPassword(passwordModel.Password, usersRole.PasswordHash ?? throw new Exception("Unable to verify password")))
                {
                    throw new Exception("Invalid password");
                }
            });
        }

        public async Task<IEnumerable<Claim>> GenerateLogin(HttpResponse httpResponse, UserIdentifierModel userIdentifierModel, Action<User>? validate = null)
        {
            var user = await _userRepository.GetUser(userIdentifierModel) ?? throw new Exception("User not found");

            if (validate is not null)
                validate(user);

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
                using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

                await _userRepository.AssignRoleToUser(conn, userIdentifierModel, roleIdModel);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to assign user to role: {ex.Message}");
            }
        }
    }
}
