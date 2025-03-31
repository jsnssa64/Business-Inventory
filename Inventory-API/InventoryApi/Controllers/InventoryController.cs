using Domain.Inventory;
using EventStore.Client;
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

        [HttpPost(Name = "AddInventoryAsync")]
        public async Task<IActionResult> Add(InventoryItem inventoryItem)
        {
            var restockedEvent = new InventoryItemRestocked
            {
                InventoryItemId = inventoryItem.ItemId,
                Quantity = inventoryItem.Quantity
            };

            try
            {
                await _inventoryService.AddInventoryItem(restockedEvent, "InventoryItemRestocked", 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding inventory");
                return StatusCode(500);
            }

            return Ok();
        }

        [HttpPost(Name = "RemoveInventoryAsync")]
        public async Task<IActionResult> Remove(InventoryItem inventoryItem)
        {
            var removeEvent = new InventoryItemRemoved
            {
                InventoryItemId = inventoryItem.ItemId,
                Quantity = inventoryItem.Quantity
            };

            try
            {
                await _inventoryService.RemoveInventoryItem(removeEvent, "InventoryItemRemoved", 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing inventory");
                return StatusCode(500);
            }

            return Ok();
        }

        [HttpPost(Name = "RemoveInventoryAsync")]
        public async Task<IEnumerable<ResolvedEvent>> GetAll(CancellationToken cancellationToken)
        {
            try
            {
                return await _inventoryService.GetInventoryItems(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all inventory");
                return Enumerable.Empty<ResolvedEvent>();
            }
        }
    }
}
