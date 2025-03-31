using EventStore.Client;

namespace InventoryApi.Service
{
    public interface IInventoryService
    {
        Task<List<ResolvedEvent>> GetInventoryItems(CancellationToken cancellationToken);
        Task<bool> RemoveInventoryItem(object eventObject, string type, long intialPos);
        Task<bool> AddInventoryItem(object eventObject, string type, long intialPos);
    }
}