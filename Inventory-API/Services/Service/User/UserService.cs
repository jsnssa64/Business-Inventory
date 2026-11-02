using System.Security.Claims;
using ZiggyCreatures.Caching.Fusion;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Http;
using Shared.Utilities.User;
using Domain.ValueObjects.User;
using Microsoft.Extensions.Logging;
using Domain.Interface;
using Services.Service.Security.Utilities;
using Services.Service.Security;
using Services.Service.Security.Models;
using Domain.Events.User;
using Services.Service.User.Input;
using Services.Service.User.Dto;
using Domain.ValueObjects.User.Role;
using Domain.ValueObjects.User.Role.Extensions;

namespace Services.Service.User
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
        private ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository,
            IPublishEndpoint publishEndpoint,
            IMediator mediator,
            ISecurityService securityService,
            IUserUtility userUtility,
            IJWTUtility jwtUtility,
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
        }

        public async Task RegisterUser(HttpResponse httpResponse, UserIdInput userId, UserRegistrationInput userRegistrationDetails)
        {
            await CreateUser(httpResponse, userId, userRegistrationDetails);
        }

        public async Task RegisterDefaultUser(HttpResponse httpResponse, UserIdInput userId, UserRegistrationInput userRegistrationDetails)
        {
            userRegistrationModel.RoleName = UserRoles.DefaultRole.ToString();
            await CreateUser(httpResponse, userId, userRegistrationDetails);
        }


        private async Task<UserIdDto> CreateUser(HttpResponse httpResponse, UserRegistrationInput userRegistrationDetails)
        {
            var encryptedPassword = await _securityService.HashPassword(userRegistrationDetails.Password, SecurityLevel.Mid);
            
            var userPassword = new UserPassword(encryptedPassword);
            var userRole = UserRole.GetUserRole(userRegistrationDetails.Role);

            var userDetails = new UserDetails()
            {
                FirstName = userRegistrationDetails.FirstName,
                LastName = userRegistrationDetails.LastName,
                ContactNumber = userRegistrationDetails.ContactNumber,
                DOB = userRegistrationDetails.DOB,
                Gender = userRegistrationDetails.Gender,
                UserAddress = new UserAddress()
                {
                    AddressLine1 = userRegistrationDetails.AddressLine1,
                    AddressLine2 = userRegistrationDetails.AddressLine2,
                    City = userRegistrationDetails.City,
                    State = userRegistrationDetails.State,
                    Country = userRegistrationDetails.Country,
                    PostalCode = userRegistrationDetails.PostalCode
                }
            };

            var userId = await _userRepository.Create(userDetails, userRole, userPassword);

            await _userRepository.AssignRoleToUser(userId, userRole);

            //  TODO: This line will auto activate the User - TEMPORARY until email confirmation is implemented
            await _userRepository.ActivateUser(userId);

            await _mediator.Publish(
                new UserCreatedNotification()
                {
                    Version = 1,
                    UserIdentity = new UserIdentity(userId.ToString(), "", userRegistrationDetails.Email),
                }
            );

            //  Auto login user after registration
            await LoginUser(httpResponse, new UserLoginModel()
            {
                Username = userId.Username,
                Password = userRegistrationDetails.Password
            });

            return new UserIdDto(userId.ToString());
        }

        private async Task ActivateUser(UserId userId)
        { 
            await _userRepository.ActivateUser(userId);
        }

        public async Task UpdateUser(UserIdInput userIdInput, UserDetailsModel userDetailsModel)
        {
            await _userRepository.UpdateUserDetails(userIdInput, userDetailsModel);
        }

        public async Task SetUserStatus(UserIdInput userIdInput, StatusModel statusModel)
        {
            await _userRepository.SetUserStatus(userIdInput, statusModel);
        }

        public async Task UserConfirmation(string token)
        {
            if (!_jwtUtility.IsTokenValid(token, KeyType.confirmation))
                throw new Exception("Confirmation Token no longer valid");

            var tokenClaims = await _jwtUtility.GetTokenClaims(token, KeyType.confirmation);

            var userId = new UserId(_userUtility.GetClaimForUser(tokenClaims.Claims, UserClaim.UserId));

            await ActivateUser(userId);
        }

        public async Task ResetPassword(string token, PasswordModel passwordModel)
        {
            var tokenClaims = await _jwtUtility.GetTokenClaims(token, KeyType.resetPassword);

            var userIdentifier = new UserId(_userUtility.GetClaimForUser(tokenClaims.Claims, UserClaim.UserId));

            await _userRepository.ResetPassword(userIdentifier, passwordModel);
        }

        public async Task ChangePassword(UserIdentifierModel userIdentifierModel, PasswordModel passwordModel)
        {
            var user = await _userRepository.GetUser(userIdentifierModel);

            if (!_securityService.VerifyPassword(passwordModel.OldPassword, user.PasswordHash))
            {
                throw new Exception("Invalid password");
            }

            passwordModel.NewPassword = _securityService.HashPassword(passwordModel.NewPassword, SecurityLevel.Mid);

            await _userRepository.ResetPassword(userIdentifierModel, passwordModel);
        }

        private async Task LoginUser(HttpResponse httpResponse, UserLoginModel userLoginModel)
        {
            var userIdentifierModel = new UserIdentifierModel() { 
                Username = userLoginModel.Username 
            };

            await GenerateLogin(httpResponse, userIdentifierModel, (usersRole) =>
            {
                if (!_securityService.VerifyPassword(userLoginModel.Password, usersRole.PasswordHash))
                {
                    throw new Exception("Invalid password");
                }
            });
        }

        private async Task<IEnumerable<Claim>> GenerateLogin(HttpResponse httpResponse, UserIdentifierModel userIdentifierModel, Action<UserWithPassword>? validate = null)
        {
            var user = await _userRepository.GetUser(userIdentifierModel);

            if (validate is not null)
                validate(user);

            var claims = _userUtility.MapUserToClaims(user.User);

            var cookieExpiry = DateTimeOffset.UtcNow.AddDays(1);

            var tokens = _securityService.GenerateLoginJWT(claims, claims);

            _securityService.SetCookieForLogin(httpResponse, tokens.AccessToken, tokens.RefreshToken, cookieExpiry);

            return claims;
        }

        private async Task<UserDetails> GetUserDetails(UserIdentifierModel userName)
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

        public async Task ForgottenPasswordByUsername(UserIdentifierModel userIdentifierModel)
        {
            await _userRepository.IsValidUserByUsername(userIdentifierModel)) ;

            var user = await _userRepository.GetUser(userIdentifierModel);
                
            var token = await _securityService.GenerateUserJWT(userIdentifierModel, KeyType.resetPassword);

            await _mediator.Publish(new PasswordResetRequestedNotification()
            {
                Username = user.Username,
                Email = user.Email,
                Token = token
            });

            var user = await _userRepository.GetUser(userIdentifierModel);
                
            var token = await _securityService.GenerateUserJWT(userIdentifierModel, KeyType.resetPassword);

            try 
            { 
                await _mediator.Publish(new EmailConfirmationModel(user.Username, user.Email, token));
            }
            catch
            {
                //  Handle Gracefully - Do not fail the user request if email fails to send
                _logger.LogWarning("Failed to send email for user: {Username}", user.Username);
            }
        }

        public void LogoutUser(HttpResponse httpResponse)
        {
            _securityService.SetCookieForLogout(httpResponse);
        }

        public async Task AssignUserToRole(UserIdentifierModel userIdentifierModel, RoleNameModel roleIdModel)
        {
            await _userRepository.AssignRoleToUser(userIdentifierModel, roleIdModel);
        }
    }
}
