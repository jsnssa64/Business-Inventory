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
using Services.Service.User.Input;
using Services.Service.User.Dto;
using Domain.ValueObjects.User.Role;

namespace Services.Service.User
{
    public class UserService : IUserService
    {
        private readonly IMediator _mediator;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IFusionCache _fusionCache;
        private readonly IUserRepository _userRepository;
        private readonly ISecurityService _securityService;
        private readonly IUserUtility _userUtility;
        private readonly IJWTUtility _jwtUtility;
        private readonly ILogger<UserService> _logger;

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

        public async Task RegisterUser(HttpResponse httpResponse, UserIdInput userId, UserDetailsInput userDetailsInput)
        {
            await CreateUser(httpResponse, userDetailsInput);
        }

        public async Task RegisterDefaultUser(HttpResponse httpResponse, UserIdInput userId, UserDetailsInput userDetailsInput)
        {
            await CreateUser(httpResponse, userDetailsInput);
        }


        private async Task<UserIdDto> CreateUser(HttpResponse httpResponse, UserDetailsInput userRegistrationDetails)
        {
            var encryptedPassword = await _securityService.HashPassword(userRegistrationDetails.Password, SecurityLevel.Mid);
            
            var userEncryptedPassword = new UserEncryptedPassword(encryptedPassword);
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

            var userId = await _userRepository.Create(userDetails, userRole, userEncryptedPassword);

            await _userRepository.AssignRoleToUser(userId, userRole);

            //  TODO: This line will auto activate the User - TEMPORARY until email confirmation is implemented
            await _userRepository.ActivateUser(userId);

            //await _mediator.Publish(
            //    new UserCreatedNotification()
            //    {
            //        Version = 1,
            //        UserIdentity = new UserIdentity(userId, "", userRegistrationDetails.Email),
            //    }
            //);

            //  Auto login user after registration
            await LoginUser(httpResponse, userId, new UserPassword(userRegistrationDetails.Password));

            return new UserIdDto(userId.ToString());
        }

        private async Task ActivateUser(UserId userId)
        { 
            await _userRepository.ActivateUser(userId);
        }

        public async Task UpdateUser(UserIdInput userIdInput, UserDetailsInput userDetailsModel)
        {
            var userDetails = new UserDetails()
            {
                FirstName = userDetailsModel.FirstName,
                LastName = userDetailsModel.LastName,
                ContactNumber = userDetailsModel.ContactNumber,
                DOB = userDetailsModel.DOB,
                Gender = userDetailsModel.Gender,
                UserAddress = new UserAddress()
                {
                    AddressLine1 = userDetailsModel.AddressLine1,
                    AddressLine2 = userDetailsModel.AddressLine2,
                    City = userDetailsModel.City,
                    State = userDetailsModel.State,
                    Country = userDetailsModel.Country,
                    PostalCode = userDetailsModel.PostalCode
                }
            };

            var userId = new UserId(userIdInput.UserId);

            await _userRepository.UpdateUserDetails(userId, userDetails);
        }

        public async Task SetUserStatus(UserIdInput userIdInput, UserStatus status)
        {
            var userId = new UserId(userIdInput.UserId);
            await _userRepository.SetUserStatus(userId, status);
        }

        public async Task UserConfirmation(string token)
        {
            if (!_jwtUtility.IsTokenValid(token, KeyType.confirmation))
                throw new Exception("Confirmation Token no longer valid");

            var tokenClaims = await _jwtUtility.GetTokenClaims(token, KeyType.confirmation);

            var userId = new UserId(_userUtility.GetClaimForUser(tokenClaims.Claims, UserClaim.UserId));

            await ActivateUser(userId);
        }

        //  <summary>
        //      User has forgotten password and is resetting it using the token sent to their email
        //  </summary>
        public async Task ResetPassword(string token, UserPasswordInput password)
        {
            var tokenClaims = await _jwtUtility.GetTokenClaims(token, KeyType.resetPassword);

            var userId = new UserId(_userUtility.GetClaimForUser(tokenClaims.Claims, UserClaim.UserId));

            var passwordHash = new UserEncryptedPassword(await _securityService.HashPassword(password.NewPassword, SecurityLevel.Mid));

            await _userRepository.SetPassword(userId, passwordHash);
        }

        //  <summary>
        //      User explicitly asks to change password
        //  </summary>
        public async Task ChangePassword(UserIdInput userIdInput, UserChangePasswordInput passwordChangeInput)
        {
            var userId = new UserId(userIdInput.UserId);
            var user = await _userRepository.GetUser(userId);

            if (!await _securityService.VerifyPassword(passwordChangeInput.OldPassword, user.Password!.PasswordHash))
            {
                throw new Exception("Invalid password");
            }

            var passwordHash = new UserEncryptedPassword(await _securityService.HashPassword(passwordChangeInput.NewPassword, SecurityLevel.Mid));

            await _userRepository.SetPassword(userId, passwordHash);
        }

        private async Task LoginUser(HttpResponse httpResponse, UserId userId, UserPassword userPassword)
        {
            await GenerateLogin(httpResponse, userId, async (user) =>
            {
                if (!await _securityService.VerifyPassword(userPassword.Password, user.Password!.PasswordHash))
                {
                    throw new Exception("Invalid password");
                }
            });
        }

        private async Task<IEnumerable<Claim>> GenerateLogin(HttpResponse httpResponse, UserId userId, Action<UserDetails>? validate = null)
        {
            var user = await _userRepository.GetUser(userId);

            if (validate is not null)
                validate(user);

            var claims = _userUtility.MapUserToClaims(user);

            var cookieExpiry = DateTimeOffset.UtcNow.AddDays(1);

            (var accessToken, var refreshToken) = await _securityService.GenerateLoginJWT(claims, claims);

            await _securityService.SetCookieForLogin(httpResponse, accessToken, refreshToken, cookieExpiry);

            return claims;
        }

        private async Task<UserDetails> GetUserDetails(UserId userId)
        {
            var userDetails = await _fusionCache
                                    .GetOrSetAsync<UserDetails>($"UserDetail-{userId}", 
                                        async (ctx, ct) =>
                                        {
                                            return await _userRepository.GetUserDetails(userId);
                                        },
                                        options: new FusionCacheEntryOptions().SetDuration(new TimeSpan(100000)));
            return userDetails;
        }

        public async Task<UserIdDto> GetUser(UserIdInput userIdInput)
        {
            var user = await _fusionCache
                                    .GetOrSetAsync<UserDetails>($"User-{userIdInput.UserId}",
                                        async (ctx, ct) =>
                                        {
                                            return await _userRepository.GetUser(new UserId(userIdInput.UserId));
                                        },
                                        options: new FusionCacheEntryOptions().SetDuration(new TimeSpan(1000)));

            return new UserIdDto(user.UserIdentity!.Id.Id);
        }

        public async Task<IEnumerable<UserDetailsDto>> GetUsers()
        {
            var users = await _userRepository.GetAllUsers();
            return users.Select(u => new UserDetailsDto(u!.UserIdentity.Id.Id, u.UserIdentity.Username, u.UserIdentity.Email));
        }

        public async Task ForgottenPasswordByEmail(UserEmailInput userEmailInput)
        {
            var user = await _userRepository.GetUserByEmail(userEmailInput.Email);

            var token = await _securityService.GenerateUserJWT(new UserIdInput(user!.UserIdentity.Id.Id), KeyType.resetPassword);

            //  Send to rabbitmq
            //  Send Email Password Reset Notification
            //await _mediator.Publish(new PasswordResetRequestedNotification()
            //{
            //    Username = user.Username,
            //    Email = user.Email,
            //    Token = token
            //});
        }

        public async Task ForgottenPasswordByUsername(UserIdInput userIdInput)
        {
            var userId = new UserId(userIdInput.UserId);
            await _userRepository.IsValidUserByUsername(userId) ;

            var user = await _userRepository.GetUser(userId);
                
            var token = await _securityService.GenerateUserJWT(userIdInput, KeyType.resetPassword);

            //  Send to rabbitmq
            //  Send Email Password Reset Notification
            //await _mediator.Publish(new PasswordResetRequestedNotification()
            //{
            //    Username = user.Username,
            //    Email = user.Email,
            //    Token = token
            //});
        }

        public async Task LogoutUser(HttpResponse httpResponse)
        {
            await _securityService.SetCookieForLogout(httpResponse);
        }

        public async Task AssignUserToRole(UserIdInput userIdInput, UserRoleInput roleInput)
        {
            var userId = new UserId(userIdInput.UserId);
            var userRole = UserRole.GetUserRole(roleInput.Role);

            await _userRepository.AssignRoleToUser(userId, userRole);
        }
    }
}
