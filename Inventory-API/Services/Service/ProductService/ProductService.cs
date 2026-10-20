using System.Data.Entity.Infrastructure;
using System.Data;
using Services.Repository.Inventory;
using Microsoft.IdentityModel.Tokens;
using MediatR;
using Services.DataModel.Product;
using Services.DataModel.User;
using Microsoft.Extensions.Logging;
using Services.Repository.Product;
using Domain.Entities.Product;
using Services.DataModel.Inventory;

namespace Services.Service.ProductService
{
    public class ProductService(
            IProductRepository productRepository,
            IMediator mediator,
            IInventoryRepository inventoryRepository,
            ILogger<ProductService> logger,
            IDbConnectionFactory dbConnectionFactory): IProductService
    {
        private readonly IMediator _mediator = mediator;
        private readonly ILogger<ProductService> _logger = logger;
        private readonly IProductRepository _productRepository = productRepository;
        private readonly IInventoryRepository _inventoryRepository = inventoryRepository;
        private readonly IDbConnectionFactory _dbConnectionFactory = dbConnectionFactory;

        public async Task<ProductIdModel> AddAsync(UserIdentifierModel userIdentifierModel, ProductDetailsModel productDetailsModel, PriceModel? priceModel, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userIdentifierModel.Username))
            {
                throw new Exception("User cannot be null or empty");
            }

            if (productDetailsModel.EnabledPrice)
            {
                if (priceModel is null)
                    throw new Exception("Product is price enabled, missing price model");

                if (priceModel.Price == 0)
                {
                    throw new Exception("Price cannot be 0");
                }

                if (priceModel.CurrencyCode.IsNullOrEmpty())
                {
                    throw new Exception("Quantity cannot be 0");
                }
            }

            using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

            conn.Open();
            var transaction = conn.BeginTransaction();

            try
            {
                var productResult = await _productRepository.AddProduct(conn, userIdentifierModel, productDetailsModel, transaction, cancellationToken);

                if (productDetailsModel.EnabledPrice && priceModel is not null)
                {
                    var productIdentifier = new ProductIdentifierModel()
                    {
                        PublicProductId = productResult.PublicProductId,
                        Username = userIdentifierModel.Username
                    };

                    await _productRepository.AddPriceToProduct(conn, productIdentifier, priceModel, transaction, cancellationToken);
                }

                var inventoryItemModel = new InventoryItemModel()
                {
                    Quantity = productDetailsModel.InventoryQuantity < 0 ? 0 : productDetailsModel.InventoryQuantity
                };

                var productIdentifierModel = new ProductIdentifierModel()
                {
                    PublicProductId = productResult.PublicProductId,
                    Username = userIdentifierModel.Username,
                };

                await _inventoryRepository.UpdateItemInInventory(productIdentifierModel, inventoryItemModel, conn, transaction, cancellationToken);

                transaction.Commit();

                //  TO DO: Publish Action to Event
                //await _mediator.Publish(new InventoryAdded(default, new(new ProductIdentity(productResult.PublicProductId), Guid.NewGuid()), default));

                return productResult;
            }
            finally
            {
                if (transaction?.Connection != null)
                    transaction.Rollback();

                transaction?.Dispose();
                conn.Close();
            }
        }

        public async Task<ProductEntity> GetAsync(ProductIdentifierModel productIdentifierModel, CancellationToken cancellationToken)
        {
            var result = await _productRepository.GetProductById(productIdentifierModel, cancellationToken);

            return result;
        }

        public async Task<IEnumerable<ProductEntity>> GetByUserAsync(UserIdentifierModel userIdentifierModel, CancellationToken cancellationToken)
        {
            return await _productRepository.GetProducts(userIdentifierModel, cancellationToken);
        }

        public async Task UpdateAsync(ProductIdentifierModel productIdentifierModel, UpdateProductDetailsModel updateProductDetailsModel, CancellationToken cancellationToken)
        {
            await _productRepository.UpdateProduct(productIdentifierModel, updateProductDetailsModel, cancellationToken);
        }

        public async Task DeleteAsync(ProductIdentifierModel productIdentifierModel, CancellationToken cancellationToken)
        {
            await _productRepository.RemoveProductById(productIdentifierModel, cancellationToken);
        }
    }
}
