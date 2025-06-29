using Domain.Entities.Inventory;
using Domain.User;
using InventoryApi.Repository.Data.User;
using Services.DataModel.Inventory;
using Services.DataModel.Product;
using Services.DataModel.User;

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