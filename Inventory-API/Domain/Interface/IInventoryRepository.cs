using System.Data;
using Domain.Entities.Inventory;
using Domain.ValueObjects.Product;
using Domain.ValueObjects.User;

namespace Domain.Interface
{
    public interface IInventoryRepository
    {
        Task<InventoryItem> GetInventoryItemByProductId(ProductIdentity productIdentifierModel, CancellationToken cancellationToken);
        Task<IEnumerable<InventoryItem>> GetInventoryItems(UserIdentity userIdentifierModel, CancellationToken cancellationToken);
        Task UpdateItemToInventoryByProductIdTransact(IDbConnection dbConnection, ProductIdentity productIdentifier, InventoryItem inventoryItemModel, CancellationToken cancellationToken, IDbTransaction? dbTransaction);
        Task UpdateItemToInventoryByProductId(ProductIdentity productIdentifier, InventoryItem inventoryItemModel, CancellationToken cancellationToken);
    }
}
