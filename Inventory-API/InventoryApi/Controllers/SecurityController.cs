using InventoryApi.Controllers.Base;
using InventoryApi.DTOs.User;
using Microsoft.AspNetCore.Mvc;
using Services.Service.SecurityService;

namespace InventoryApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SecurityController(ISecurityService securityService) : BaseController
    {
        private readonly ISecurityService _securityService = securityService;

        [HttpPost("CreatePassword")]
        //  DevOnly - TODO
        public IActionResult CreatePassword(UserNewPassword userNewPassword, CancellationToken cancellationToken)
        {
            var passwordHash = _securityService.HashPassword(userNewPassword.NewPassword);
            return Ok(passwordHash);
        }
    }
}
