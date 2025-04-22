using System.Data;
using Domain.Inventory;
using EventStore.Client;
using InventoryApi.Repository.Data.Inventory;
using InventoryApi.Repository.Data.Product;

namespace InventoryApi.Repository.Inventory
{
    public interface IInventoryRepository
    {
        Task<IWriteResult> AppendEventStream(object eventObject, string type, long intialPos);
        Task<InventoryItem> GetInventoryItemByProductId(ProductIdentifierModel productIdentifierModel);
        Task<IEnumerable<InventoryItem>> GetInventoryItems(UserIdentifierModel userIdentifierModel);
        Task<List<ResolvedEvent>> ReadEventStream(CancellationToken cancellationToken);
        Task UpdateItemToInventoryByProductIdTransact(IDbConnection dbConnection, ProductIdentifierModel productIdentifier, InventoryItemModel inventoryItemModel, IDbTransaction? dbTransaction);
        Task UpdateItemToInventoryByProductId(ProductIdentifierModel productIdentifier, InventoryItemModel inventoryItemModel);
    }
}