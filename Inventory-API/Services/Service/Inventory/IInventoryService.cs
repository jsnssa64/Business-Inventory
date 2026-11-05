using Domain.Entities.Inventory;
using Domain.ValueObjects.Product;
using Domain.ValueObjects.User;

namespace Services.Service.Inventory
{
    public interface IInventoryService
    {
        Task<InventoryItem> GetByProductId(ProductIdentity productId, CancellationToken cancellationToken);
        Task<IEnumerable<InventoryItem>> GetItems(UserIdentity userId, CancellationToken cancellationToken);
        Task UpdateItemByProductId(ProductIdentity productId, InventoryItem item, CancellationToken cancellationToken);
    }
}
