using InventoryApi.Controllers.CustomController;
using InventoryApi.Model.DTO.User;
using InventoryApi.Repository.Data.Product;
using InventoryApi.Repository.Data.Role;
using InventoryApi.Repository.Data.User;
using InventoryApi.Service.SecurityService;
using InventoryApi.Service.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
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
        public async Task<IActionResult> AssignUserRole(UsersRoleDTO usersRoleDTO, CancellationToken cancellationToken)
        {
            await _userService.AssignUserToRole(
                new UserIdentifierModel()
                {
                    Username = usersRoleDTO.UserName,
                },
                new RoleIdModel()
                {
                    PublicRoleId = usersRoleDTO.RoleId
                });
            return Ok();
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(UserNewPassword userNewPassword, CancellationToken cancellationToken)
        {
            var passwordModel = new PasswordModel()
            {
                Password = userNewPassword.NewPassword
            };
            var userIdentifierModel = new UserIdentifierModel()
            {
                Username = GetUsername()
            };

            await _userService.ChangePassword(userIdentifierModel, passwordModel);
            return Ok();
        }

        [HttpGet("Confirmation")]
        [AllowAnonymous]
        public async Task<IActionResult> Confirmation(string token, CancellationToken cancellationToken)
        {
            if (!_jwtUtility.IsTokenValid(token, KeyType.confirmation))
                throw new Exception("Confirmation no longer valid");

            await _userService.UserConfirmation(token);
            return Ok();
        }

        [HttpGet("Disable")]
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

        [HttpPost("ForgottenPasswordByEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgottenPasswordByEmail(UserEmailDTO userEmailDTO, CancellationToken cancellationToken)
        {
            var userEmailModel = new UserEmailModel() { Email = userEmailDTO.Email };
            await _userService.ForgottenPasswordByEmail(userEmailModel);
            return Ok();
        }

        [HttpPost("ForgottenPasswordByUsername")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgottenPasswordByUsername(UsernameDTO usernameDTO, CancellationToken cancellationToken)
        {
            var userIdentifierModel = new UserIdentifierModel() { Username = usernameDTO.UserName };

            await _userService.ForgottenPasswordByUsername(userIdentifierModel);
            return Ok();
        }

        [HttpGet("GetUserDetailsByUser")]
        public async Task<IActionResult> GetUserDetails(string username, CancellationToken cancellationToken)
        {
            var allUserData = await _userService.GetUserDetails(new UserIdentifierModel() { Username = username });
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
            var passwordModel = new PasswordModel()
            {
                Password = userLoginDto.UserPassword
            };
            var userIdentifierModel = new UserIdentifierModel()
            {
                Username = userLoginDto.UserName
            };

            await _userService.LoginUser(Response, userIdentifierModel, passwordModel);
            return Ok();
        }

        [HttpGet("Logout")]
        [AllowAnonymous]
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
        public async Task<IActionResult> RegisterUserWithRole(UserWithRoleRegisterDTO userWithRoleRegisterDTO, CancellationToken cancellationToken)
        {
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
                    PublicRoleId = userWithRoleRegisterDTO.RoleId
                });
            return Ok();
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO resetPasswordDTO, CancellationToken cancellationToken)
        {
            var passwordModel = new PasswordModel()
            {
                Password = resetPasswordDTO.newPassword
            };

            await _userService.ResetPassword(resetPasswordDTO.token, passwordModel);
            return Ok();
        }

        [HttpPost("Update")]
        [AllowAnonymous]
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
