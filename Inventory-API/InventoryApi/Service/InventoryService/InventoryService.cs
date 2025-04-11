using Domain.Inventory;
using EventStore.Client;
using InventoryApi.Repository.Inventory;
using InventoryApi.Repository.Model;
using Microsoft.IdentityModel.Tokens;

namespace InventoryApi.Service.InventoryService
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

        public async Task<Product> AddInventoryItem(Product product, InventoryItem inventory, CancellationToken cancellationToken)
        {
            if (product.Price == 0)
            {
                throw new Exception("Price cannot be 0");
            }

            if (product.Quantity == 0) { 
                throw new Exception("Quantity cannot be 0");
            }

            var result = await _inventoryRepository.AddProduct(product);

            if (inventory.InventoryQuantity > 0)
            {
                inventory.ProductId = result.Id;
                await _inventoryRepository.AddItemToInventoryByProductId(inventory);
            }

            return result;
        }

        public async Task<bool> AddItemToInventory(Product product, InventoryItem inventoryItem, CancellationToken cancellationToken)
        {
            if (inventoryItem.InventoryQuantity == 0)
            { 
                throw new Exception("Inventory Quantity cannot be 0");
            }

            if(string.IsNullOrEmpty(product.Name))
            {
                throw new Exception("Product Name cannot be empty or null");
            }

            return await _inventoryRepository.AddItemToInventoryByName(product, inventoryItem);
        }

        public async Task<IEnumerable<InventoryInfo>> GetInventoryItemByProductId(Product product, CancellationToken cancellationToken)
        {
            if(product.Name.IsNullOrEmpty())
            {
                throw new Exception("Item Name cannot be 0");
            }

            return await _inventoryRepository.GetInventoryInfoByItemName(product);
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
