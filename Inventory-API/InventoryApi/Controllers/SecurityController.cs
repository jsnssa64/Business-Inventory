using InventoryApi.Controllers.CustomController;
using InventoryApi.DTOs.User;
using Microsoft.AspNetCore.Mvc;
using Services.Service.SecurityService;

namespace InventoryApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SecurityController(ISecurityService securityService) : BaseController
    {
        public ISecurityService SecurityService { get; } = securityService;

        [HttpPost("CreatePassword")]
        //  DevOnly - TODO
        public IActionResult CreatePassword(UserNewPassword userNewPassword, CancellationToken cancellationToken)
        {
            var passwordHash = SecurityService.EncryptPassword(userNewPassword.NewPassword);
            return Ok(passwordHash);
        }
    }
}
