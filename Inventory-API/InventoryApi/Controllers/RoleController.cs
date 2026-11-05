using InventoryApi.DTOs.RoleDTO;
using Microsoft.AspNetCore.Mvc;
using InventoryApi.Controllers.HostedSupplier;
using InventoryApi.Authorization;
using Domain.ValueObjects.User.Role;
using Services.Service.Role;

namespace InventoryApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [MinimumRole(UserRoleLevel.Admin)]
    public class RoleController : ControllerBase
    {
        private readonly ILogger<InventoryController> _logger;
        private readonly IRoleService _roleService;

        public RoleController(ILogger<InventoryController> logger, IRoleService roleService)
        {
            _logger = logger;
            _roleService = roleService;
        }

        [Obsolete]
        private async Task<IActionResult> CreateRole(CreateRoleDTO createRoleDTO)
        {
            throw new NotImplementedException();
        }

        [HttpGet("GetRoles")]
        public IActionResult GetRoles()
        {
            throw new NotImplementedException();
        }

        [HttpGet("GetDefaultRole")]
        public IActionResult GetDefaultRole()
        {
            throw new NotImplementedException();
        }
    }
}
