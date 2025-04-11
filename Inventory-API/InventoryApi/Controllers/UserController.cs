using Domain.Inventory;
using Domain.User;
using EventStore.Client;
using InventoryApi.Model.DTO;
using InventoryApi.Model.Events.Inventory;
using InventoryApi.Service;
using InventoryApi.Service.InventoryService;
using Microsoft.AspNetCore.Mvc;

namespace InventoryApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<InventoryController> _logger;
        private readonly IInventoryService _inventoryService;

        public UserController(ILogger<InventoryController> logger, IInventoryService inventoryService)
        {
            _logger = logger;
            _inventoryService = inventoryService;
        }

        [HttpGet("Login")]
        public async Task<IActionResult> UserLogin(User user, CancellationToken cancellationToken)
        {
            return Ok();

        }

        [HttpPost("AddInventoryItem")]
        public async Task<IActionResult> AddInventoryItem(InventoryItemDTO dto, CancellationToken cancellationToken)
        {
            try
            {
                if (dto == null)
                {
                    throw new ArgumentNullException(nameof(dto));
                }

                var inventoryItem = new Product
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    Price = dto.Price,
                    CurrencyCode = dto.Currency,
                    Quantity = dto.ItemQuantity
                };

                var inventory = new InventoryItem
                {
                    Quantity = dto.InventoryQuantity
                };

                var result = await _inventoryService.AddInventoryItem(inventoryItem, inventory, cancellationToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding inventory");
                return StatusCode(500);
            }
        }

        [HttpPost("AddItemToInventory")]
        public async Task<IActionResult> AddItemToInventory(AddToInventoryDTO dto, CancellationToken cancellationToken)
        {
            try
            {
                if (dto == null)
                {
                    throw new ArgumentNullException(nameof(dto));
                }

                var inventoryItem = new Product
                {
                    Name = dto.Name
                };

                var inventory = new InventoryItem
                {
                    Quantity = dto.InventoryQuantity
                };

                var result = await _inventoryService.AddItemToInventory(inventoryItem, inventory, cancellationToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding item to inventory");
                return StatusCode(500);
            }
        }

        [HttpPost("AddInventoryToStream")]
        public async Task<IActionResult> AddToStream(Product inventoryItem)
        {
            var restockedEvent = new InventoryItemRestocked
            {
                InventoryItemId = inventoryItem.ItemId,
                Quantity = inventoryItem.Quantity
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
        public async Task<IActionResult> RemoveFromStream(InventoryItem item)
        {
            var removeEvent = new InventoryItemRemoved
            {
                InventoryItemId = item.ItemId,
                Quantity = item.Quantity
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
