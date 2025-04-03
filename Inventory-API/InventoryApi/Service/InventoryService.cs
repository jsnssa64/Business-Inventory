using Domain.Inventory;
using EventStore.Client;
using InventoryApi.Repository;

namespace InventoryApi.Service
{
    public class InventoryService: IInventoryService
    {
        private ILogger<InventoryService> _logger;
        private IInventoryRepository _inventoryRepository;

        public InventoryService(IInventoryRepository inventoryRepository, ILogger<InventoryService> logger)
        {
            _logger = logger;
            _inventoryRepository = inventoryRepository;
        }

        public async Task<InventoryItem> AddInventoryItem(InventoryItem inventoryItem, Inventory inventory, CancellationToken cancellationToken)
        {
            if (inventoryItem.Price == 0)
            {
                throw new Exception("Price cannot be 0");
            }

            if (inventoryItem.Quantity == 0) { 
                throw new Exception("Quantity cannot be 0");
            }

            var result = await _inventoryRepository.AddInventoryItem(inventoryItem);

            if (inventory.Quantity > 0)
            {
                inventory.ItemId = result.ItemId;
                await _inventoryRepository.AddItemToInventoryById(inventory);
            }

            return result;
        }

        public async Task<bool> AddItemToInventory(InventoryItem inventoryItem, Inventory inventory, CancellationToken cancellationToken)
        {
            if (inventory.Quantity == 0)
            { 
                throw new Exception("Quantity cannot be 0");
            }

            if(String.IsNullOrEmpty(inventoryItem.Name))
            {
                throw new Exception("Item Name cannot be empty or null");
            }

            return await _inventoryRepository.AddItemToInventoryByName(inventoryItem, inventory);
        }

        public async Task<IEnumerable<InventoryItem>> GetInventoryItemByItemId(InventoryItem inventoryItem, CancellationToken cancellationToken)
        {
            if(inventoryItem.ItemId == 0)
            {
                throw new Exception("Item Id cannot be 0");
            }

            return await _inventoryRepository.GetInventoryItemByItemId(inventoryItem);
        }   

        public async Task<bool> RemoveInventoryItemFromStream(object eventObject, string type, long intialPos)
        {
            try
            {
                await _inventoryRepository.AppendEventStream(eventObject, type, intialPos);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing inventory");
                return false;
            }
        }

        public async Task<bool> AddInventoryItemToStream(object eventObject, string type, long intialPos)
        {
            try
            {
                await _inventoryRepository.AppendEventStream(eventObject, type, intialPos);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error add inventory");
                return false;
            }
        }
    }
}
