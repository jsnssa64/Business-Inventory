using System.Data;
using System.Security.Claims;
using ZiggyCreatures.Caching.Fusion;
using MassTransit;
using MediatR;
using Services.DataModel.Role;
using Services.DataModel.User;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Shared.Utilities.User;
using Services.Repository.UserRepo;
using Shared.Constants;
using Domain.ValueObjects.User;
using Domain.Service.UserService;
using Services.Service.SecurityService;
using Services.Service.SecurityService.Models;
using Services.Service.UserService.Notification;
using Microsoft.Extensions.Logging;

namespace Services.Service.UserService
{
    public class UserService : IUserService
    {
        private IMediator _mediator;
        private IPublishEndpoint _publishEndpoint;
        private IFusionCache _fusionCache;
        private IUserRepository _userRepository;
        private ISecurityService _securityService;
        private IUserUtility _userUtility;
        private IJWTUtility _jwtUtility;
        private IDbConnectionFactory _dbConnectionFactory;
        private ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository,
            IPublishEndpoint publishEndpoint,
            IMediator mediator,
            ISecurityService securityService, 
            IUserUtility userUtility, 
            IJWTUtility jwtUtility,
            IDbConnectionFactory dbConnectionFactory,
            IFusionCache fusionCache,
            ILogger<UserService> logger) 
        {
            _mediator = mediator;
            _publishEndpoint = publishEndpoint;
            _fusionCache = fusionCache;
            _userRepository = userRepository;
            _logger = logger;
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
                userRegistrationModel.RoleName = Roles.DefaultRole.ToString();
                await this.CreateUser(httpResponse, userIdentifierModel, userRegistrationModel);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to register new User: {ex.Message}");
            }
        }

        private async Task<UserIdModel> CreateUser(HttpResponse httpResponse, UserIdentifierModel userIdentifierModel, UserRegistrationModel userRegistrationModel)
        {
            if(!Roles.IsValidRoleLevel(userRegistrationModel.RoleName))
            {
                throw new Exception("Invalid Role");
            }

            using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());
            conn.Open();
            var transaction = conn.BeginTransaction();

            try
            {
                userRegistrationModel.EncryptedPassword = _securityService.EncryptPassword(userRegistrationModel.Password, SecurityLevel.Mid);

                var userIdModel = await _userRepository.CreateUser(conn, userIdentifierModel, userRegistrationModel, transaction);

                var roleNameModel = new RoleNameModel
                {
                    RoleName = userRegistrationModel.RoleName.ToString()
                };

                await _userRepository.AssignRoleToUser(conn, userIdentifierModel, roleNameModel, transaction);

                //  TODO: This line will auto activate the User - TEMPORARY until email confirmation is implemented
                await _userRepository.ActivateUser(conn, userIdentifierModel, transaction);

                transaction.Commit();

                try
                {
                    await _mediator.Publish(
                        new UserCreatedNotification()
                        {
                            Version = 1,
                            userIdentity = new UserIdentity(userRegistrationModel.UserId.ToString(), "", userRegistrationModel.Email),
                        }
                    );
                }
                catch (Exception ex)
                {
                    //  Log failed notification, but do not fail the user registration
                    _logger.LogError($"Failed to publish {nameof(UserCreatedNotification)}: {ex.Message}");
                }

                //  Auto login user after registration
                await LoginUser(httpResponse, new UserLoginModel()
                {
                    Username = userIdentifierModel.Username,
                    Password = userRegistrationModel.Password
                });
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
                throw new Exception($"Failed to activate User: {ex.Message}");
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
                throw new Exception($"Failed to set User Status {ex.Message}");
            }
        }

        public async Task UserConfirmation(string token)
        {
            if (!_jwtUtility.IsTokenValid(token, KeyType.confirmation))
                throw new Exception("Confirmation Token no longer valid");

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

        public async Task ChangePassword(UserIdentifierModel userIdentifierModel, PasswordModel passwordModel)
        {
            try
            {
                var user = await _userRepository.GetUser(userIdentifierModel);

                if (!_securityService.VerifyPassword(passwordModel.OldPassword, user.PasswordHash))
                {
                    throw new Exception("Invalid password");
                }

                passwordModel.NewPassword = _securityService.EncryptPassword(passwordModel.NewPassword);

                await _userRepository.ResetPassword(userIdentifierModel, passwordModel);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to Reset User password: {ex.Message}");
            }
        }

        public async Task LoginUser(HttpResponse httpResponse, UserLoginModel userLoginModel)
        {
            var userIdentifierModel = new UserIdentifierModel() { 
                Username = userLoginModel.Username 
            };

            await this.GenerateLogin(httpResponse, userIdentifierModel, (usersRole) =>
            {
                if (!_securityService.VerifyPassword(userLoginModel.Password, usersRole.PasswordHash ?? throw new Exception("Unable to verify password")))
                {
                    throw new Exception("Invalid password");
                }
            });
        }

        public async Task<IEnumerable<Claim>> GenerateLogin(HttpResponse httpResponse, UserIdentifierModel userIdentifierModel, Action<UserWithPassword>? validate = null)
        {
            var user = await _userRepository.GetUser(userIdentifierModel);

            if (validate is not null)
                validate(user);

            var claims = _userUtility.MapUserToClaims(user);

            var cookieExpiry = DateTimeOffset.UtcNow.AddDays(1);

            var tokens = _securityService.GenerateLoginJWT(claims, claims);

            _securityService.SetCookieForLogin(httpResponse, tokens.AccessToken, tokens.RefreshToken, cookieExpiry);

            return claims;
        }

        public async Task<UserDetails> GetUserDetails(UserIdentifierModel userName)
        {
            var userDetails = await _fusionCache
                                    .GetOrSetAsync<UserDetails>($"UserDetail-{userName.Username}", 
                                        async (ctx, ct) =>
                                        {
                                            return await _userRepository.GetUserDetails(userName);
                                        },
                                        options: new FusionCacheEntryOptions().SetDuration(new TimeSpan(100000)));

            if (userDetails is null)
                throw new Exception("Not valid userdetails");

            return userDetails;
        }

        public async Task<User> GetUser(UserIdentifierModel userName)
        {
            var user = await _fusionCache
                                    .GetOrSetAsync<User>($"User-{userName.Username}",
                                        async (ctx, ct) =>
                                        {
                                            return await _userRepository.GetUser(userName);
                                        },
                                        options: new FusionCacheEntryOptions().SetDuration(new TimeSpan(1000)));

            return user;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            return await _userRepository.GetAllUsers();
        }

        public async Task ForgottenPasswordByEmail(UserEmailModel userEmailModel)
        {
            try
            {
                var user = await _userRepository.GetUserByEmail(userEmailModel);

                var userIdentifierModel = new UserIdentifierModel()
                {
                    Username = user.Username
                };

                var token = await _securityService.GenerateUserJWT(userIdentifierModel, KeyType.resetPassword);

                //  Send to rabbitmq
                await _mediator.Publish(new PasswordResetRequestedNotification()
                {
                    Username = user.Username,
                    Email = user.Email,
                    Token = token
                });
            }
            catch(Exception ex)
            {
                throw new Exception("Failed to send email", ex);
            } 
        }

        public async Task ForgottenPasswordByUsername(UserIdentifierModel userIdentifierModel)
        {
            try
            {
                if (!await _userRepository.IsValidUserByUsername(userIdentifierModel))
                {
                    throw new Exception("Invalid Username");
                }

                var user = await _userRepository.GetUser(userIdentifierModel);
                
                var token = await _securityService.GenerateUserJWT(userIdentifierModel, KeyType.resetPassword);

                await _mediator.Publish(new PasswordResetRequestedNotification()
                {
                    Username = user.Username,
                    Email = user.Email,
                    Token = token
                });
            }
            catch(Exception ex)
            {
                throw new Exception("Failed to send email", ex);
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

        public async Task AssignUserToRole(UserIdentifierModel userIdentifierModel, RoleNameModel roleIdModel)
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
