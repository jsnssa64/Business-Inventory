using Domain.Entities.Inventory;
using InventoryApi.Repository.Inventory;
using Microsoft.Extensions.Logging;
using Services.DataModel.Inventory;
using Services.DataModel.Product;
using Services.DataModel.User;

namespace Services.Service.InventoryService
{
    public class InventoryService : IInventoryService
    {
        private ILogger<InventoryService> _logger;
        private IInventoryRepository _inventoryRepository;

        public InventoryService(IInventoryRepository inventoryRepository, ILogger<InventoryService> logger)
        {
            _logger = logger;
            _inventoryRepository = inventoryRepository;
        }

        public async Task UpdateItemToInventoryByProductId(ProductIdentity productIdentifierModel, InventoryItemModel inventoryItemModel, CancellationToken cancellationToken)
        {
            if (inventoryItemModel == null)
            {
                throw new ArgumentNullException(nameof(inventoryItemModel));
            }

            await _inventoryRepository.UpdateItemToInventoryByProductId(productIdentifierModel, inventoryItemModel);
        }

        public async Task<InventoryItem> GetInventoryItemByProductId(ProductIdentity productIdentifierModel, CancellationToken cancellationToken)
        {
            return await _inventoryRepository.GetInventoryItemByProductId(productIdentifierModel);
        }

        public async Task<IEnumerable<InventoryItem>> GetInventoryItems(UserIdentifierModel userIdentifierModel, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userIdentifierModel.Username))
            {
                throw new Exception("Username cannot be null or empty");
            }

            return await _inventoryRepository.GetInventoryItems(userIdentifierModel);
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
