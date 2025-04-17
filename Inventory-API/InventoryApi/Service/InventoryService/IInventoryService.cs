using Domain.Inventory;
using Domain.User;
using InventoryApi.Repository.Data.Inventory;
using InventoryApi.Repository.Data.Product;
using InventoryApi.Repository.Data.User;

namespace InventoryApi.Service.InventoryService
{
    public interface IInventoryService
    {
        Task<bool> AddInventoryItemToStream(object eventObject, string type, long intialPos);
        Task<InventoryItem> GetInventoryItemByProductId(ProductIdentifierModel productIdentifierModel, CancellationToken cancellationToken);
        Task<IEnumerable<InventoryItem>> GetInventoryItems(UserIdentifierModel userIdentifierModel, CancellationToken cancellationToken);
        Task<bool> RemoveInventoryItemFromStream(object eventObject, string type, long intialPos);
        Task UpdateItemToInventoryByProductId(ProductIdentifierModel productIdentifierModel, InventoryItemModel inventoryItemModel, CancellationToken cancellationToken);
    }
}