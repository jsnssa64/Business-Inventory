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

        public async Task<List<ResolvedEvent>> GetInventoryItems(CancellationToken cancellationToken)
        {
            return await _inventoryRepository.ReadEventStream(cancellationToken);
        }   

        public async Task<bool> RemoveInventoryItem(object eventObject, string type, long intialPos)
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

        public async Task<bool> AddInventoryItem(object eventObject, string type, long intialPos)
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
