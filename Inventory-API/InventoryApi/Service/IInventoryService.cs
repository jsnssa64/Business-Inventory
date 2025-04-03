using Domain.Inventory;

namespace InventoryApi.Service
{
    public interface IInventoryService
    {
        //Task<List<ResolvedEvent>> GetInventoryItems(CancellationToken cancellationToken);
        Task<bool> RemoveInventoryItemFromStream(object eventObject, string type, long intialPos);
        Task<bool> AddInventoryItemToStream(object eventObject, string type, long intialPos);
        Task<InventoryItem> AddInventoryItem(InventoryItem inventoryItem, Inventory inventory, CancellationToken cancellationToken);
        Task<bool> AddItemToInventory(InventoryItem inventoryItem, Inventory inventory, CancellationToken cancellationToken);
        Task<IEnumerable<InventoryItem>> GetInventoryItemByItemId(InventoryItem inventoryItem, CancellationToken cancellationToken);
    }
}