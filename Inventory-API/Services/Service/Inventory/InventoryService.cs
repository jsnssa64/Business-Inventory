using Domain.Entities.Inventory;
using Domain.Entities.Product;
using Domain.Events.Inventory;
using Domain.Events.Inventory.Core;
using Domain.Interface;
using Domain.ValueObjects.Inventory;
using Domain.ValueObjects.Product;
using Domain.ValueObjects.User;
using Microsoft.Extensions.Logging;

namespace Services.Service.Inventory
{
    public class InventoryService : IInventoryService
    {
        private readonly ILogger<InventoryService> _logger;
        private readonly IInventoryRepository _inventoryRepository;

        public InventoryService(IInventoryRepository inventoryRepository, ILogger<InventoryService> logger)
        {
            _logger = logger;
            _inventoryRepository = inventoryRepository;
        }

        public async Task<ProductIdentity> AddInventoryItem(UserIdentity UserIdentifierModel, ProductInfo info, Price? priceModel, CancellationToken cancellationToken)
        {
            var inventoryItem = new InventoryItem();
            inventoryItem.UpdateQuantity(product.Quantity);


            await _inventoryRepository.UpdateItemToInventoryByProductIdTransact(productId, inventoryItem, cancellationToken);

            await _mediator.Publish(new InventoryAdded()
            {
                InventoryEvent = new InventoryEvent(
                    1,
                    new InventoryItemIdentity()
                    {
                        InventoryId = default,
                        productIdentity = new ProductIdentity()
                        {
                            PublicProductId = productResult.PublicProductId,

                        }
                    }),
                Quantity = inventoryItemModel.Quantity
            }, CancellationToken.None);
        }


        public async Task UpdateItemToInventoryByProductId(ProductIdentity productId, InventoryItem item, CancellationToken cancellationToken)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            await _inventoryRepository.UpdateItemToInventoryByProductId(productId, item, cancellationToken);
        }

        public async Task<InventoryItem> GetInventoryItemByProductId(ProductIdentity productId, CancellationToken cancellationToken)
        {
            return await _inventoryRepository.GetInventoryItemByProductId(productId, cancellationToken);
        }

        public async Task<IEnumerable<InventoryItem>> GetInventoryItems(UserIdentity userId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userId.Username))
            {
                throw new Exception("Username cannot be null or empty");
            }

            return await _inventoryRepository.GetInventoryItems(userId, cancellationToken);
        }
    }
}
