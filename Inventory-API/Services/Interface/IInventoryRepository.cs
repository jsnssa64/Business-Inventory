using System.Data;
using Domain.Entities.Inventory;
using Domain.ValueObjects.Product;
using KurrentDB.Client;
using Domain.ValueObjects.User;

namespace Services.Interface
{
    public interface IInventoryRepository
    {
        Task<IWriteResult> AppendEventStream(object eventObject, string type, long intialPos);
        Task<InventoryItem> GetInventoryItemByProductId(ProductIdentity productIdentifierModel);
        Task<IEnumerable<InventoryItem>> GetInventoryItems(UserIdentity userIdentifierModel);
        Task<List<ResolvedEvent>> ReadEventStream(CancellationToken cancellationToken);
        Task UpdateItemToInventoryByProductIdTransact(IDbConnection dbConnection, ProductIdentity productIdentifier, InventoryItemModel inventoryItemModel, IDbTransaction? dbTransaction);
        Task UpdateItemToInventoryByProductId(ProductIdentity productIdentifier, InventoryItemModel inventoryItemModel);
    }
}
