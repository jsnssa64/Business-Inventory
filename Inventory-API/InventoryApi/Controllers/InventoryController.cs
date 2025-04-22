using Domain.Inventory;
using Domain.User;
using InventoryApi.Authentication;
using InventoryApi.Controllers.CustomController;
using InventoryApi.Model.DTO.Inventory;
using InventoryApi.Model.Events.Inventory;
using InventoryApi.Repository.Data.Inventory;
using InventoryApi.Repository.Data.Product;
using InventoryApi.Service.InventoryService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Domain.User.Roles;

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
                var productIdentifier = new ProductIdentifierModel
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

                var productIdentifier = new ProductIdentifierModel()
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
            var restockedEvent = new InventoryItemRestocked
            {
                ProductId = product.Id,
                InventoryItemId = inventoryItem.Id,
                Quantity = product.Quantity
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
            var removeEvent = new InventoryItemRemoved
            {
                ProductId = product.Id,
                InventoryItemId = inventoryItem.Id,
                Quantity = product.Quantity
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
