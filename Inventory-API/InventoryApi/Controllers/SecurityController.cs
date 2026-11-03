using InventoryApi.Controllers.Base;
using InventoryApi.DTOs.User;
using Microsoft.AspNetCore.Mvc;
using Services.Service.Security;

namespace InventoryApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SecurityController(ISecurityService securityService) : BaseController
    {
        private readonly ISecurityService _securityService = securityService;

        [HttpPost("CreatePassword")]
        //  DevOnly - TODO
        public async Task<IActionResult> CreatePassword(UserNewPasswordDTO userNewPassword, CancellationToken cancellationToken)
        {
            var passwordHash = await _securityService.HashPassword(userNewPassword.NewPassword);
            return Ok(passwordHash);
        }
    }
}
