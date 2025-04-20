using Domain.User;
using InventoryApi.Model.DTO.RoleDTO;
using InventoryApi.Model.DTO.User;
using InventoryApi.Repository.Data;
using InventoryApi.Repository.Data.Role;
using InventoryApi.Service.RoleService;
using InventoryApi.Service.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly ILogger<InventoryController> _logger;
        private readonly IRoleService _roleService;

        public RoleController(ILogger<InventoryController> logger, IRoleService roleService)
        {
            _logger = logger;
            _roleService = roleService;
        }

        [HttpGet("CreateRole")]
        public async Task<IActionResult> CreateRole(CreateRoleDTO createRoleDTO)
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
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _roleService.GetRoles();
            return Ok(roles);
        }

        [HttpGet("GetDefaultRole")]
        public async Task<IActionResult> GetDefaultRole()
        {
            var role = await _roleService.GetDefaultRole();
            return Ok(role);
        }
    }
}
