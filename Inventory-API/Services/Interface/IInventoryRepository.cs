using System.Data;
using Domain.Entities.Inventory;
using KurrentDB.Client;

namespace Services.Interface.Inventory
{
    public interface IInventoryRepository
    {
        Task<IWriteResult> AppendEventStream(object eventObject, string type, long intialPos);
        Task<InventoryItem> GetInventoryItemByProductId(ProductIdentity productIdentifierModel);
        Task<IEnumerable<InventoryItem>> GetInventoryItems(UserIdentifierModel userIdentifierModel);
        Task<List<ResolvedEvent>> ReadEventStream(CancellationToken cancellationToken);
        Task UpdateItemToInventoryByProductIdTransact(IDbConnection dbConnection, ProductIdentity productIdentifier, InventoryItemModel inventoryItemModel, IDbTransaction? dbTransaction);
        Task UpdateItemToInventoryByProductId(ProductIdentity productIdentifier, InventoryItemModel inventoryItemModel);
    }
}