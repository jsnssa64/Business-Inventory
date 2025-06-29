using System.Data;
using Domain.Entities.Inventory;
using KurrentDB.Client;
using Services.DataModel.Inventory;
using Services.DataModel.Product;
using Services.DataModel.User;

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