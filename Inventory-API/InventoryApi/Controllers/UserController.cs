using System.CodeDom;
using System.Security.Claims;
using InventoryApi.Controllers.CustomController;
using InventoryApi.Model.DTO.User;
using InventoryApi.Repository.Data.Product;
using InventoryApi.Repository.Data.Role;
using InventoryApi.Repository.Data.User;
using InventoryApi.Service.SecurityService;
using InventoryApi.Service.UserService;
using InventoryApi.Service.UserService.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
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
        private readonly IUserUtility _userUtility;

        public UserController(ILogger<InventoryController> logger, IUserService userService, IJWTUtility jwtUtility, IUserUtility userUtility)
        {
            _logger = logger;
            _userService = userService;
            _jwtUtility = jwtUtility;
            _userUtility = userUtility;
        }

        [HttpGet("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUser(UserRegisterDTO userRegisterDTO,  CancellationToken cancellationToken)
        {
            await _userService.RegisterDefaultUser(Response, new UserIdentifierModel() { 
                Username = userRegisterDTO.Username,
            }, 
            new UserRegistrationModel() {
                FirstName = userRegisterDTO.FirstName,
                LastName = userRegisterDTO.LastName,
                Email = userRegisterDTO.Email,
                Password = userRegisterDTO.Password
            });
            return Ok();
        }

        [HttpGet("RegisterRole")]
        public async Task<IActionResult> RegisterUserWithRole(UserWithRoleRegisterDTO userWithRoleRegisterDTO, CancellationToken cancellationToken)
        {
            await _userService.RegisterUser(Response, new UserIdentifierModel()
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


        [HttpGet("Update")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateUser(UpdateUserDetailsDTO updateUserDetailsDTO, CancellationToken cancellationToken)
        {
            await _userService.UpdateUser(new UserIdentifierModel() { 
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


        [HttpGet("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginUser(UserLoginDTO userLoginDto, CancellationToken cancellationToken)
        {
            var passwordModel = new PasswordModel()
            {
                Password = userLoginDto.UserPassword
            };

            var userIdentifierModel = new UserIdentifierModel()
            {
                Username = GetUsername()
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

        [HttpGet("Disable")]
        [AllowAnonymous]
        public IActionResult DisableUser(CancellationToken cancellationToken)
        {
            var userIdentifier = new UserIdentifierModel() { Username = GetUsername() };
            var statusModel = new StatusModel() {  Enabled = false };
            _userService.SetUserStatus(userIdentifier, statusModel);
            return Ok();
        }

        [HttpGet("Confirmation")]
        [AllowAnonymous]
        public async Task<IActionResult> Confirmation(TokenDTO tokenDTO, CancellationToken cancellationToken)
        {
            if(!_jwtUtility.IsTokenValid(tokenDTO.token, KeyType.confirmation))
                throw new Exception("Confirmation no longer valid");

            var tokenClaims = await _jwtUtility.GetTokenClaims(tokenDTO.token, KeyType.confirmation);

            var userIdentifier = new UserIdentifierModel() { 
                Username = _userUtility.GetClaimForUser(tokenClaims.Claims, UserClaim.Username)
            };
            
            var statusModel = new StatusModel() { 
                Enabled = true 
            };

            await _userService.SetUserStatus(userIdentifier, statusModel);
            return Ok();
        }

        public class TokenDTO
        {
            public required string token { get; set; }
        }

        public class UserDTO
        {
            public required string Username { get; set; }
        }

        [HttpGet("DeActivateUser")]
        [AllowAnonymous]
        public IActionResult DeActivateUser(UserDTO userDTO, CancellationToken cancellationToken)
        {
            _userService.SetUserStatus(
            new UserIdentifierModel() { 
                Username = userDTO.Username 
            }, 
            new StatusModel() { 
                Enabled = false 
            });

            return Ok();
        }

        [HttpGet("AssignUserRole")]
        public async Task<IActionResult> AssignUserRole(UsersRoleDTO usersRoleDTO, CancellationToken cancellationToken)
        {
            await _userService.AssignUserToRole(
                new UserIdentifierModel() { 
                    Username = usersRoleDTO.UserName,
                }, 
                new RoleIdModel() { 
                    PublicRoleId = usersRoleDTO.RoleId
                });
            return Ok();
        }

        [HttpGet("GetUserDetailsByUser")]
        public async Task<IActionResult> GetUserDetails(UsernameDTO usernameDTO, CancellationToken cancellationToken)
        {
            var allUserData = await _userService.GetUserDetails(new UserIdentifierModel() { Username = usernameDTO.UserName });
            return Ok(allUserData);
        }

        [HttpGet("GetUserDetails")]
        public async Task<IActionResult> GetUserDetails(CancellationToken cancellationToken)
        {
            var allUserData = await _userService.GetUserDetails(new UserIdentifierModel() { Username = GetUsername() });
            return Ok(allUserData);
        }
    }
}
