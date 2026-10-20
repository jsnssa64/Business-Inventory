using System.Data;
using Domain.Entities.Inventory;
using Services.DataModel.Inventory;
using Services.DataModel.Product;
using Services.DataModel.User;

namespace Services.Repository.Inventory
{
    public interface IInventoryRepository
    {
        Task UpdateItemInInventory(ProductIdentifierModel productIdentifier, InventoryItemModel inventoryItemModel, CancellationToken cancellationToken);
        Task UpdateItemInInventory(ProductIdentifierModel productIdentifier, InventoryItemModel inventoryItemModel, IDbConnection dbConnection, IDbTransaction? dbTransaction, CancellationToken cancellationToken);
        Task<InventoryItem?> GetInventoryItemByProductId(ProductIdentifierModel productIdentifierModel, CancellationToken cancellationToken);
        Task<IEnumerable<InventoryItem>> GetInventoryItems(UserIdentifierModel userIdentifierModel, CancellationToken cancellationToken);
    }
}
