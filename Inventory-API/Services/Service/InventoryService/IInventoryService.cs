using Domain.Entities.Inventory;
using Domain.Entities.Product;
using Services.DataModel.Inventory;
using Services.DataModel.Product;
using Services.DataModel.User;

namespace Services.Service.InventoryService
{
    public interface IInventoryService
    {
        Task<bool> AddInventoryItemToStream(object eventObject, string type, long intialPos);
        Task<InventoryItem> GetInventoryItemByProductId(ProductIdentity productIdentifierModel, CancellationToken cancellationToken);
        Task<IEnumerable<InventoryItem>> GetInventoryItems(UserIdentifierModel userIdentifierModel, CancellationToken cancellationToken);
        Task<bool> RemoveInventoryItemFromStream(object eventObject, string type, long intialPos);
        Task UpdateItemToInventoryByProductId(ProductIdentity productIdentifierModel, InventoryItemModel inventoryItemModel, CancellationToken cancellationToken);
    }
}