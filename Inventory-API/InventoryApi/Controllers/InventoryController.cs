using InventoryApi.Authentication;
using InventoryApi.DTOs.Inventory;
using InventoryApi.Model.Events.Inventory;
using Microsoft.AspNetCore.Mvc;
using Services.DataModel.Inventory;
using Services.DataModel.Product;
using Services.DataModel.User;
using Services.Service.InventoryService;
using Domain.ValueObjects.User;

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

        [HttpGet("GetInventoryItemByProductId")]
        public async Task<IActionResult> GetInventoryItemByProductId(Guid productid, CancellationToken cancellationToken)
        {
            try
            {
                var productIdentifier = new ProductIdentity
                {
                    PublicProductId = productid,
                    Username = GetUsername()
                };

                var inventoryItems = await _inventoryService.GetInventoryItemByProductId(productIdentifier, cancellationToken);

                return Ok(inventoryItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding inventory");
                return StatusCode(500);
            }
        }

        [HttpGet("GetInventory")]
        public async Task<IActionResult> GetInventory(CancellationToken cancellationToken)
        {
            try
            {
                var userIdentifier = new UserIdentifierModel() { 
                    Username = GetUsername() 
                };

                var inventoryItems = await _inventoryService.GetInventoryItems(userIdentifier, cancellationToken);

                return Ok(inventoryItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding inventory");
                return StatusCode(500);
            }
        }

        [HttpPost("UpdateItemInInventory")]
        public async Task<IActionResult> UpdateItemInInventory(UpdateInventoryItemDTO updateInventoryItemDTO, CancellationToken cancellationToken)
        {
            try
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

                await _inventoryService.UpdateItemToInventoryByProductId(productIdentifier, inventoryItemModel, cancellationToken);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding item to inventory");
                return StatusCode(500);
            }
        }

        [HttpPost("AddInventoryToStream")]
        [Obsolete]
        private async Task<IActionResult> AddToStream(Product product, InventoryItem inventoryItem)
        {
            var restockedEvent = new InventoryAdded
            {
                InventoryEvent =  new InventoryEvent(default, new InventoryItemIdentity()
                {
                    // Temp
                    InventoryId = new Guid(),
                    productIdentity = new ProductIdentity()
                    {
                        publicProductId = product.PublicProductId.publicProductId
                    }
                 }),
                Quantity = inventoryItem.Quantity,
                Version = 1
                
            };

            var inventoryItems = await _inventoryService.GetInventoryItems(userIdentifier, cancellationToken);

            return Ok(inventoryItems);
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
