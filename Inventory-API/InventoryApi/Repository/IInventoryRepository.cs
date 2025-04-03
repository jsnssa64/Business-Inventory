using Domain.Inventory;
using EventStore.Client;
using InventoryApi.Repository.Model;

namespace InventoryApi.Repository
{
    public interface IInventoryRepository
    {
        Task<IEnumerable<InventoryItem>> GetInventoryItemByItemId(InventoryItem inventoryItem);
        Task<bool> AddItemToInventoryById(Inventory inventory);
        Task<bool> AddItemToInventoryByName(InventoryItem inventoryItem, Inventory inventory);
        Task<InventoryItem> AddInventoryItem(InventoryItem inventoryItem);
        Task<IEnumerable<InventoryInfo>> GetInventoryInfoByItemName(InventoryItem inventoryItem);
        Task<List<ResolvedEvent>> ReadEventStream(CancellationToken cancellationToken);
        Task<IWriteResult> AppendEventStream(object eventObject, string type, long intialPos);
    }
}