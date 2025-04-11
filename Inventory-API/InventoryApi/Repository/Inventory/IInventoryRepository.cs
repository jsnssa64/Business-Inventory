using Domain.Inventory;
using EventStore.Client;
using InventoryApi.Repository.Model;

namespace InventoryApi.Repository.Inventory
{
    public interface IInventoryRepository
    {
        Task<bool> AddItemToInventoryByName(Product product, InventoryItem inventoryItem);
        Task<bool> AddItemToInventoryByProductId(InventoryItem inventoryItem);
        Task<Product> AddProduct(Product product);
        Task<IWriteResult> AppendEventStream(object eventObject, string type, long intialPos);
        Task<IEnumerable<InventoryInfo>> GetInventoryInfoByItemName(Product product);
        Task<IEnumerable<Product>> GetProductById(Product product);
    }
}