using Domain.User;
using InventoryApi.Model.DTO.RoleDTO;
using InventoryApi.Model.DTO.User;
using InventoryApi.Repository.Data;
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
        [AllowAnonymous]
        public async Task<IActionResult> CreateRole(RoleIdOnlyDTO roleIdOnlyDTO)
        {
            var role = await _roleService.CreateRole(roleIdOnlyDTO.RolePublicId);
            return Ok(role);
        }
    }
}
