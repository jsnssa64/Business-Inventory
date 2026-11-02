using Domain.ValueObjects.User.Role;
using InventoryApi.Authorization;
using InventoryApi.Controllers.Base;
using InventoryApi.DTOs.Inventory;
using Microsoft.AspNetCore.Mvc;
using Services.Service.Inventory;

namespace InventoryApi.Controllers.HostedSupplier
{
    [ApiController]
    [Route(V)]
    [MinimumRole(UserRoleLevel.Supplier)]
    public class InventoryController(IInventoryService inventoryService) : BaseController
    {
        private readonly IInventoryService _inventoryService = inventoryService;

        [HttpGet("Inventory/{itemId}")]
        public async Task<IActionResult> GetInventoryItem(string itemId, CancellationToken cancellationToken)
        {
            if(!Guid.TryParse(itemId, out var inventoryItemGuid))
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
                Username = GetUsername() 
            };

            var inventoryItems = await _inventoryService.GetInventoryItems(userIdentifier, cancellationToken);

            return Ok(inventoryItems);
        }

        [HttpPost("Inventory")]
        public async Task<IActionResult> UpdateItemInInventory(UpdateInventoryItemDTO updateInventoryItemDTO, CancellationToken cancellationToken)
        {
            var inventoryItemModel = new InventoryItemModel
            {
                Quantity = updateInventoryItemDTO.Quantity
            };

            var productIdentifier = new ProductIdentity()
            {
                PublicProductId = updateInventoryItemDTO.ProductId,
                Username = GetUsername()
            };

            await _inventoryService.UpdateItemByProductId(productIdentifier, inventoryItemModel, cancellationToken);
            return Ok();
        }

        [HttpPatch("Inventory/{inventoryItemId}")]
        public async Task<IActionResult> UpdateInventory(string inventoryItemId, UpdateInventoryItemDTO updateInventoryItemDTO, CancellationToken cancellationToken)
        {
            var removeEvent = new InventoryRemoved
            {
                Quantity = inventoryItem.Quantity,
                InventoryEvent = new InventoryEvent(
                    1, 
                    new InventoryItemIdentity()
                    {
                        InventoryId = new Guid(),
                        productIdentity = new ProductIdentity()
                        {
                            publicProductId = product.PublicProductId.publicProductId
                        }
                    }),
                Version = 1
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
