using System.Data.Entity.Infrastructure;
using System.Data;
using InventoryApi.Repository.Data.Inventory;
using InventoryApi.Repository.Inventory;
using Microsoft.IdentityModel.Tokens;
using MassTransit;
using MassTransit.Transports;
using MediatR;
using InventoryApi.Model.Events.Inventory;
using InventoryApi.Constants;
using Services.DataModel.Product;
using Services.DataModel.User;
using Domain.Entities.Product;

namespace InventoryApi.Service.InventoryService
{
    public class ProductService: IProductService
    {
        private IMediator _mediator;
        private ILogger<ProductService> _logger;
        private IProductRepository _productRepository;
        private IInventoryRepository _inventoryRepository;
        private IDbConnectionFactory _dbConnectionFactory;

        public ProductService(IProductRepository productRepository, 
            IMediator mediator,
            IInventoryRepository inventoryRepository, 
            ILogger<ProductService> logger,
            IDbConnectionFactory dbConnectionFactory)
        {
            _mediator = mediator;
            _logger = logger;
            _productRepository = productRepository;
            _inventoryRepository = inventoryRepository;
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<ProductIdModel> AddProductAsync(UserIdentifierModel userIdentifierModel, ProductDetailsModel productDetailsModel, PriceModel? priceModel)
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
                var productResult = await _productRepository.AddProduct(conn, userIdentifierModel, productDetailsModel, transaction);

                if (productDetailsModel.EnabledPrice && priceModel is not null)
                {
                    var productIdentifier = new ProductIdentifierModel()
                    {
                        PublicProductId = productResult.PublicProductId,
                        Username = userIdentifierModel.Username
                    };

                    await _productRepository.AddPriceToProduct(conn, productIdentifier, priceModel, transaction);
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

                await _inventoryRepository.UpdateItemToInventoryByProductIdTransact(conn, productIdentifierModel, inventoryItemModel, transaction);

                transaction.Commit();

                await _mediator.Publish(new InventoryAdded(default, null, default)
                {
                    ProductId = productResult.PublicProductId,
                    Quantity = inventoryItemModel.Quantity
                });

                return productResult;
            }
            catch (Exception ex)
            {
                if (transaction?.Connection != null)
                    transaction.Rollback();



                throw new Exception("Failed to add product", ex);
            }
        }

        public async Task<Product> GetProductByIdAsync(ProductIdentifierModel productIdentifierModel, CancellationToken cancellationToken)
        {
            var result = await _productRepository.GetProductById(productIdentifierModel);

            return result;
        }

        public async Task<IEnumerable<ProductBase>> GetProducts(UserIdentifierModel userIdentifierModel, CancellationToken cancellationToken)
        {
            return await _productRepository.GetProducts(userIdentifierModel);
        }

        public async Task UpdateProduct(ProductIdentifierModel productIdentifierModel, UpdateProductDetailsModel updateProductDetailsModel, CancellationToken cancellationToken)
        {
            await _productRepository.UpdateProduct(productIdentifierModel, updateProductDetailsModel);
        }

        public async Task RemoveProduct(ProductIdentifierModel productIdentifierModel, CancellationToken cancellationToken)
        {
            await _productRepository.RemoveProductById(productIdentifierModel);
        }
    }
}
