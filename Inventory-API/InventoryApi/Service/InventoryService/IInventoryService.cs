using Domain.Inventory;
using InventoryApi.Repository.Model;

namespace InventoryApi.Service.InventoryService
{
    public interface IInventoryService
    {
        Task<Product> AddInventoryItem(Product product, InventoryItem inventory, CancellationToken cancellationToken);
        Task<bool> AddInventoryItemToStream(object eventObject, string type, long intialPos);
        Task<bool> AddItemToInventory(Product product, InventoryItem inventoryItem, CancellationToken cancellationToken);
        Task<IEnumerable<InventoryInfo>> GetInventoryItemByProductId(Product product, CancellationToken cancellationToken);
        Task<bool> RemoveInventoryItemFromStream(object eventObject, string type, long intialPos);
    }
}