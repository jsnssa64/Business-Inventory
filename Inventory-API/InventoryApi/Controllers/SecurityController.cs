using InventoryApi.Controllers.CustomController;
using InventoryApi.Model.DTO.User;
using InventoryApi.Service.SecurityService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SecurityController(ISecurityService securityService) : BaseController
    {
        public ISecurityService SecurityService { get; } = securityService;

        [HttpPost("CreatePassword")]
        [AllowAnonymous]
        public IActionResult CreatePassword(UserNewPassword userNewPassword, CancellationToken cancellationToken)
        {
            var passwordHash = SecurityService.EncryptPassword(userNewPassword.NewPassword);
            return Ok(passwordHash);
        }
    }
}
