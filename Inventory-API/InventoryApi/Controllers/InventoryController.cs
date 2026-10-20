using InventoryApi.Authentication;
using InventoryApi.DTOs.Inventory;
using Services.DataModel.Product;
using Services.DataModel.User;
using Services.DataModel.Inventory;
using Services.Service.InventoryService;
using Microsoft.AspNetCore.Mvc;
using static Domain.User.Roles;
using InventoryApi.Controllers.Base;

namespace InventoryApi.Controllers
{
    [ApiController]
    [Route(V)]
    [MinimumRole(RoleLevel.User)]
    public class InventoryController(IInventoryService inventoryService) : BaseController
    {
        private readonly IInventoryService _inventoryService = inventoryService;

        [HttpGet("Inventory/{inventoryItemId}")]
        public async Task<IActionResult> GetInventoryItem(string inventoryItemId, CancellationToken cancellationToken)
        {
            if(!Guid.TryParse(inventoryItemId, out var inventoryItemGuid))
            {
                return BadRequest();
            }
            
            var productIdentifier = new ProductIdentifierModel
            {
                PublicProductId = inventoryItemGuid,
                Username = GetUsernameFromClaim()
            };

            var result = await _inventoryService.GetInventoryItemByProductId(productIdentifier, cancellationToken);

            return Ok(result);
        }

        [HttpGet("Inventory")]
        public async Task<IActionResult> GetInventory(CancellationToken cancellationToken)
        {
            var userIdentifier = new UserIdentifierModel() { 
                Username = GetUsernameFromClaim() 
            };

            var inventoryItems = await _inventoryService.GetInventoryItems(userIdentifier, cancellationToken);

            return Ok(inventoryItems);
        }

        [HttpPatch("Inventory/{inventoryItemId}")]
        public async Task<IActionResult> UpdateInventory(string inventoryItemId, UpdateInventoryItemDTO updateInventoryItemDTO, CancellationToken cancellationToken)
        {
            if(!Guid.TryParse(inventoryItemId, out var inventoryItemGuid))
            {
                return BadRequest();
            }
            var inventoryItemModel = new InventoryItemModel
            {
                Quantity = updateInventoryItemDTO.Quantity
            };

            var productIdentifier = new ProductIdentifierModel()
            {
                PublicProductId = inventoryItemGuid,
                Username = GetUsernameFromClaim()
            };

            await _inventoryService.UpdateItemToInventoryByProductId(productIdentifier, inventoryItemModel, cancellationToken);
            
            //  How should I handle failures and whether i should just catch this in a middleware
            if(result.Status == FailedToSend)
            {
                //  Appropriate Status code
                return BadRequest();
            }
            
            return Ok();
        }
    }
}
