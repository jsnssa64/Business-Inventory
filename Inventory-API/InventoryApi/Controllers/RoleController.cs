using InventoryApi.Authentication;
using InventoryApi.DTOs.RoleDTO;
using Microsoft.AspNetCore.Mvc;
using Services.DataModel.Role;
using Services.Service.RoleService;
using Domain.ValueObjects.User;

namespace InventoryApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [MinimumRole(RoleLevel.User)]
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
            var createRoleModel = new CreateRoleModel()
            {
                IsDefault = createRoleDTO.SetAsDefault,
                RoleName = createRoleDTO.RoleName
            };

            var role = await _roleService.CreateRole(createRoleModel);
            return Ok(role);
        }

        [HttpGet("GetRoles")]
        public IActionResult GetRoles()
        {
            var roles = _roleService.GetRoles();
            return Ok(roles);
        }

        [HttpGet("GetDefaultRole")]
        public IActionResult GetDefaultRole()
        {
            var role = _roleService.GetDefaultRole();
            return Ok(role);
        }
    }
}
