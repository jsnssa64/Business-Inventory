using Domain.Entities.Inventory;
using Domain.Entities.Product;
using InventoryApi.Authentication;
using InventoryApi.Controllers.CustomController;
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
    [Route("[controller]")]
    [MinimumRole(RoleLevel.User)]
    public class InventoryController : BaseController
    {
        private readonly IInventoryService _inventoryService;
        private readonly ILogger<InventoryController> _logger;

        public InventoryController(IInventoryService inventoryService, ILogger<InventoryController> logger)
        {
            _inventoryService = inventoryService;
            _logger = logger;
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

            try
            {
                await _inventoryService.AddInventoryItemToStream(restockedEvent, "InventoryItemRestocked", 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding inventory");
                return StatusCode(500);
            }

            return Ok();
        }

        [HttpPost("RemoveInventoryFromStream")]
        [Obsolete]
        private async Task<IActionResult> RemoveFromStream(Product product, InventoryItem inventoryItem)
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

            try
            {
                await _inventoryService.RemoveInventoryItemFromStream(removeEvent, "InventoryItemRemoved", 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing inventory");
                return StatusCode(500);
            }

            return Ok();
        }
    }
}
