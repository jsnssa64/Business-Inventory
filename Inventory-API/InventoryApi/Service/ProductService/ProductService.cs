using Domain.Inventory;
using Domain.User;
using InventoryApi.Repository.Data;
using InventoryApi.Repository.Data.Inventory;
using InventoryApi.Repository.Data.Product;
using InventoryApi.Repository.Data.User;
using InventoryApi.Repository.Inventory;
using Microsoft.IdentityModel.Tokens;

namespace InventoryApi.Service.InventoryService
{
    public class ProductService: IProductService
    {
        private ILogger<ProductService> _logger;
        private IProductRepository _productRepository;
        private IInventoryRepository _inventoryRepository;

        public ProductService(IProductRepository productRepository, IInventoryRepository inventoryRepository, ILogger<ProductService> logger)
        {
            _logger = logger;
            _productRepository = productRepository;
            _inventoryRepository = inventoryRepository;
        }

        public async Task<ProductIdModel> AddProductAsync(UserIdentifierModel userIdentifierModel, ProductDetailsModel productDetailsModel, PriceModel? priceModel)
        {
            if (string.IsNullOrEmpty(userIdentifierModel.Username))
            {
                throw new Exception("User cannot be null or empty");
            }

            //if(product.EnabledPrice)
            //{
            //    if (product.Price == 0)
            //    {
            //        throw new Exception("Price cannot be 0");
            //    }

            //    if (product.Quantity == 0)
            //    {
            //        throw new Exception("Quantity cannot be 0");
            //    }
            //}

            var resultId = await _productRepository.AddProduct(userIdentifierModel, productDetailsModel, priceModel);


            if (string.IsNullOrWhiteSpace(resultId.PublicProductId))
                throw new ArgumentException("Failed to public product id", nameof(resultId.PublicProductId));


            var inventoryItemModel = new InventoryItemModel()
            {
                Quantity = productDetailsModel.InventoryQuantity < 0 ? 0 : productDetailsModel.InventoryQuantity
            };

            await _inventoryRepository.UpdateItemToInventoryByProductId(productIdentifierModel, inventoryItemModel);

            return resultId;
        }

        public async Task<Product> GetProductByIdAsync(ProductIdentifierModel productIdentifierModel, CancellationToken cancellationToken)
        {
            var result = await _productRepository.GetProductById(productIdentifierModel);

            return result;
        }

        public async Task<IEnumerable<Product>> GetProducts(UserIdentifierModel userIdentifierModel, CancellationToken cancellationToken)
        {
            return await _productRepository.GetProducts(userIdentifierModel);
        }

        public async Task<Product> UpdateProduct(ProductIdentifierModel productIdentifierModel, UpdateProductDetailsModel updateProductDetailsModel, CancellationToken cancellationToken)
        {
            var result = await _productRepository.UpdateProduct(productIdentifierModel, updateProductDetailsModel);

            return result;
        }

        public async Task RemoveProduct(ProductIdentifierModel productIdentifierModel, CancellationToken cancellationToken)
        {
            await _productRepository.RemoveProductById(productIdentifierModel);
        }
    }
}
