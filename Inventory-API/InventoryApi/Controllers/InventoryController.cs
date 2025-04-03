using Domain.Inventory;
using EventStore.Client;
using InventoryApi.Model.DTO;
using InventoryApi.Model.Events.Inventory;
using InventoryApi.Service;
using Microsoft.AspNetCore.Mvc;

namespace InventoryApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;
        private readonly ILogger<InventoryController> _logger;

        public InventoryController(IInventoryService inventoryService, ILogger<InventoryController> logger)
        {
            _inventoryService = inventoryService;
            _logger = logger;
        }

        [HttpGet("GetInventoryItemById")]
        public async Task<IActionResult> GetInventoryItemById(int inventoryItemId, CancellationToken cancellationToken)
        {
            try
            {
                if (inventoryItemId == 0)
                {
                    throw new ArgumentNullException(nameof(inventoryItemId));
                }

                var inventoryItem = new InventoryItem
                {
                    ItemId = inventoryItemId
                };

                var inventoryItems = await _inventoryService.GetInventoryItemByItemId(inventoryItem, cancellationToken);

                return Ok(inventoryItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding inventory");
                return StatusCode(500);
            }
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

                var inventoryItem = new InventoryItem
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    Price = dto.Price,
                    CurrencyCode = dto.Currency,
                    Quantity = dto.ItemQuantity
                };

                var inventory = new Inventory
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

                var inventoryItem = new InventoryItem
                {
                    Name = dto.Name
                };

                var inventory = new Inventory
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
        public async Task<IActionResult> AddToStream(InventoryItem inventoryItem)
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
        public async Task<IActionResult> RemoveFromStream(InventoryItem inventoryItem)
        {
            var removeEvent = new InventoryItemRemoved
            {
                InventoryItemId = inventoryItem.ItemId,
                Quantity = inventoryItem.Quantity
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
