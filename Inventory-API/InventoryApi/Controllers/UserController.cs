using InventoryApi.Authentication;
using InventoryApi.Controllers.CustomController;
using InventoryApi.DTOs.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.DataModel.Role;
using Services.DataModel.User;
using Services.Service.SecurityService;
using Services.Service.UserService;
using Shared.Constants;

namespace InventoryApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [MinimumRole(Shared.Constants.Roles.RoleLevel.User)]
    public class UserController : BaseController
    {
        private readonly ILogger<InventoryController> _logger;
        private readonly IUserService _userService;
        private readonly IJWTUtility _jwtUtility;

        public UserController(ILogger<InventoryController> logger, IUserService userService, IJWTUtility jwtUtility)
        {
            _logger = logger;
            _userService = userService;
            _jwtUtility = jwtUtility;
        }

        [HttpPost("AssignUserRole")]
        [MinimumRole(Shared.Constants.Roles.RoleLevel.Admin)]
        public async Task<IActionResult> AssignUserRole(UsersRoleDTO usersRoleDTO, CancellationToken cancellationToken)
        {
            await _userService.AssignUserToRole(
                new UserIdentifierModel()
                {
                    Username = usersRoleDTO.UserName,
                },
                new RoleNameModel()
                {
                    RoleName = usersRoleDTO.RoleName.ToString()
                });
            return Ok();
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(UserNewPassword userNewPassword, CancellationToken cancellationToken)
        {
            var passwordModel = new PasswordModel()
            {
                NewPassword = userNewPassword.NewPassword,
                OldPassword = userNewPassword.OldPassword
            };
            var userIdentifierModel = new UserIdentifierModel()
            {
                Username = GetUsername()
            };

            await _userService.ChangePassword(userIdentifierModel, passwordModel);
            return Ok();
        }

        /*
            When User registered an email should be sent
            with a link to confirm the email address.
            The link should contain a token that is valid for a certain period.
            When the user clicks the link, the token is validated and this method is called.
         */
        [HttpGet("Confirmation")]
        [AllowAnonymous]
        public async Task<IActionResult> Confirmation(string token, CancellationToken cancellationToken)
        {

            await _userService.UserConfirmation(token);
            return Ok();
        }

        [HttpGet("Disable")]
        [MinimumRole(Shared.Constants.Roles.RoleLevel.Admin)]
        public IActionResult DisableUser(string username, CancellationToken cancellationToken)
        {
            var userIdentifier = new UserIdentifierModel()
            {
                Username = username
            };
            var statusModel = new StatusModel()
            {
                Enabled = false
            };
            _userService.SetUserStatus(userIdentifier, statusModel);
            return Ok();
        }

        [HttpGet("Enable")]
        [MinimumRole(Shared.Constants.Roles.RoleLevel.Admin)]
        public IActionResult EnableUser(string username, CancellationToken cancellationToken)
        {
            _userService.SetUserStatus(
                new UserIdentifierModel()
                {
                    Username = username
                },
                new StatusModel()
                {
                    Enabled = true
                });

            return Ok();
        }

        //  Send EMail to reset password
        [HttpPost("ForgottenPasswordByEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgottenPasswordByEmail(UserEmailDTO userEmailDTO, CancellationToken cancellationToken)
        {
            var userEmailModel = new UserEmailModel() { Email = userEmailDTO.Email };
            await _userService.ForgottenPasswordByEmail(userEmailModel);
            return Ok();
        }

        //  Send EMail to reset password
        [HttpGet("ForgottenPasswordByUsername")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgottenPasswordByUsername(string userName, CancellationToken cancellationToken)
        {
            var userIdentifierModel = new UserIdentifierModel() { 
                Username = userName 
            };

            await _userService.ForgottenPasswordByUsername(userIdentifierModel);
            return Ok();
        }

        [HttpGet("GetUserDetailsByUser")]
        [MinimumRole(Shared.Constants.Roles.RoleLevel.Admin)]
        public async Task<IActionResult> GetUserDetails(string username, CancellationToken cancellationToken)
        {
            var allUserData = await _userService.GetUserDetails(new UserIdentifierModel() { Username = username });
            return Ok(allUserData);
        }

        [HttpGet("GetUsers")]
        [MinimumRole(Shared.Constants.Roles.RoleLevel.Admin)]
        public async Task<IActionResult> GetUsers(CancellationToken cancellationToken)
        {
            var allUserData = await _userService.GetUsers();
            return Ok(allUserData);
        }

        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUser(CancellationToken cancellationToken)
        {
            var allUserData = await _userService.GetUser(new UserIdentifierModel() { Username = GetUsername() });
            return Ok(allUserData);
        }

        [HttpGet("GetUserDetails")]
        public async Task<IActionResult> GetUserDetails(CancellationToken cancellationToken)
        {
            var allUserData = await _userService.GetUserDetails(new UserIdentifierModel() { Username = GetUsername() });
            return Ok(allUserData);
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginUser(UserLoginDTO userLoginDto, CancellationToken cancellationToken)
        {
            var userLoginModel = new UserLoginModel()
            {
                Username = userLoginDto.UserName,
                Password = userLoginDto.UserPassword
            };

            await _userService.LoginUser(Response, userLoginModel);
            return Ok();
        }

        [HttpGet("Logout")]
        public IActionResult LogoutUser(CancellationToken cancellationToken)
        {
            _userService.LogoutUser(Response);
            return Ok();
        }

        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterDefaultUser(UserRegisterDTO userRegisterDTO,  CancellationToken cancellationToken)
        {
            await _userService.RegisterDefaultUser(Response, 
                new UserIdentifierModel() 
                { 
                    Username = userRegisterDTO.Username,
                }, 
                new UserRegistrationModel() 
                {
                    FirstName = userRegisterDTO.FirstName,
                    LastName = userRegisterDTO.LastName,
                    Email = userRegisterDTO.Email,
                    Password = userRegisterDTO.Password
                });
            return Ok();
        }

        [HttpPost("RegisterUserWithRole")]
        [MinimumRole(Shared.Constants.Roles.RoleLevel.Admin)]
        public async Task<IActionResult> RegisterUserWithRole(UserWithRoleRegisterDTO userWithRoleRegisterDTO, CancellationToken cancellationToken)
        {
            if (Roles.IsValidRoleLevel(userWithRoleRegisterDTO.RoleName))
            {
                return BadRequest("Invalid role name provided.");
            }

            await _userService.RegisterUser(Response,
                new UserIdentifierModel()
                {
                    Username = userWithRoleRegisterDTO.Username
                },
                new UserRegistrationModel()
                {
                    FirstName = userWithRoleRegisterDTO.FirstName,
                    LastName = userWithRoleRegisterDTO.LastName,
                    Email = userWithRoleRegisterDTO.Email,
                    Password = userWithRoleRegisterDTO.Password,
                    RoleName = userWithRoleRegisterDTO.RoleName
                });
            return Ok();
        }

        [HttpPost("ResetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO resetPasswordDTO, CancellationToken cancellationToken)
        {
            var passwordModel = new PasswordModel()
            {
                OldPassword = resetPasswordDTO.OldPassword,
                NewPassword = resetPasswordDTO.NewPassword
            };

            await _userService.ResetPassword(resetPasswordDTO.token, passwordModel);
            return Ok();
        }

        [HttpPost("Update")]
        [MinimumRole(Shared.Constants.Roles.RoleLevel.User)]
        public async Task<IActionResult> UpdateUser(UpdateUserDetailsDTO updateUserDetailsDTO, CancellationToken cancellationToken)
        {
            await _userService.UpdateUser(
                new UserIdentifierModel() 
                { 
                    Username = GetUsername()
                }, 
                new UserDetailsModel()
                {
                    FirstName = updateUserDetailsDTO.FirstName,
                    LastName = updateUserDetailsDTO.LastName,
                    Email = updateUserDetailsDTO.Email,
                    ContactNumber = updateUserDetailsDTO.ContactNumber,
                    DOB = updateUserDetailsDTO.DOB,
                    FirstLineAddress = updateUserDetailsDTO.FirstLineAddress,
                    SecondLineAddress = updateUserDetailsDTO.SecondLineAddress,
                    Country = updateUserDetailsDTO.Country,
                    Gender = updateUserDetailsDTO.Gender,
                    PostCode = updateUserDetailsDTO.PostCode
                });
            return Ok();
        }        
    }
}
