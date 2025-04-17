using System.Data;
using System.Data.Entity.Infrastructure;
using Dapper;
using Domain.Inventory;
using InventoryApi.Factory;
using InventoryApi.Model.DTO.Product;
using InventoryApi.Repository.Data;
using InventoryApi.Repository.Data.Product;
using Microsoft.IdentityModel.Tokens;

namespace InventoryApi.Repository.Inventory
{
    public class ProductRepository: IProductRepository
    {
        private IDbConnectionFactory _dbConnectionFactory;

        public ProductRepository(
            IDbConnectionFactory dbConnectionFactory
            )
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<ProductIdModel> AddProduct(ProductIdentifierModel productIdentifierModel, ProductDetailsModel productDetailsModel, PriceModel? priceModel)
        {
            using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add(nameof(ProductIdentifierModel.Username), productIdentifierModel.Username);
            parameters.Add(nameof(ProductDetailsModel.ProductName), productDetailsModel.ProductName);
            parameters.Add(nameof(ProductDetailsModel.Description), productDetailsModel.Description);
            parameters.Add(nameof(ProductDetailsModel.Quantity), productDetailsModel.Quantity);
            parameters.Add(nameof(ProductDetailsModel.EnabledPrice), productDetailsModel.EnabledPrice);
            parameters.Add(nameof(ProductIdentifierModel.PublicProductId), dbType: DbType.Guid, direction: ParameterDirection.Output);

            await conn.ExecuteAsync("dbo.AddProduct", parameters, commandType: CommandType.StoredProcedure);

            var result = new ProductIdModel()
            {
                PublicProductId = parameters.Get<string>(nameof(ProductIdModel.PublicProductId))
            };

            if(result.PublicProductId.IsNullOrEmpty())
            {
                throw new DbUpdateException($"Failed to add product {productDetailsModel.Name}");
            }

            if (priceModel is not null) {

                DynamicParameters priceParam = new DynamicParameters();
                priceParam.Add(nameof(ProductIdentifierModel.Username), productIdentifierModel.Username);
                priceParam.Add(nameof(ProductIdentifierModel.PublicProductId), result.PublicProductId);
                priceParam.Add(nameof(PriceModel.Price), priceModel.Price);
                priceParam.Add(nameof(PriceModel.CurrencyCode), priceModel.CurrencyCode);

                await conn.ExecuteAsync("dbo.AddProductPrice", parameters, commandType: CommandType.StoredProcedure);

            }

            return result;
        }

        public async Task<Product> GetProductById(ProductIdentifierModel productIdentifierModel)
        {
            using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add(nameof(ProductIdentifierModel.Username), productIdentifierModel.Username);
            parameters.Add(nameof(ProductIdentifierModel.PublicProductId), productIdentifierModel.PublicProductId);

            var result = await conn.QuerySingleAsync<Product>("dbo.GetProductById", parameters, commandType: CommandType.StoredProcedure);

            if (result is null)
            {
                throw new DbUpdateException("Failed to get product");
            }

            return result;
        }

        public async Task RemoveProductById(ProductIdentifierModel productIdentifierModel)
        {
            using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add(nameof(ProductIdentifierModel.Username), productIdentifierModel.Username);
            parameters.Add(nameof(ProductIdentifierModel.PublicProductId), productIdentifierModel.PublicProductId);

            var result = await conn.ExecuteAsync("dbo.RemoveProductById", parameters, commandType: CommandType.StoredProcedure);

            if (result != 1)
            {
                throw new DbUpdateException("Failed to remove product");
            }
        }

        public async Task<IEnumerable<Product>> GetProducts(string username)
        {
            using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("Username", username);

            var result = await conn.QueryAsync<Product>("dbo.GetProducts", parameters, commandType: CommandType.StoredProcedure);

            if (result is null)
            {
                throw new DbUpdateException("Failed to get products");
            }

            return result;
        }

        public async Task UpdateProductPrice(ProductIdentifierModel productIdentifierModel, PriceModel updatePrice)
        {
            using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add(nameof(ProductIdentifierModel.PublicProductId), productIdentifierModel.PublicProductId);
            parameters.Add(nameof(ProductIdentifierModel.Username), productIdentifierModel.Username);
            parameters.Add(nameof(PriceModel.Price), updatePrice.Price);
            parameters.Add(nameof(PriceModel.CurrencyCode), updatePrice.CurrencyCode);

            var result = await conn.ExecuteAsync("dbo.UpdateProductPrice", parameters, commandType: CommandType.StoredProcedure);

            if (result != 0)
            {
                throw new DbUpdateException("Failed to get products");
            }
        }

        public async Task<Product> UpdateProduct(ProductIdentifierModel productIdentifierModel, UpdateProductDetailsModel updateProductDetailsModel)
        {
            using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add(nameof(ProductIdentifierModel.Username), productIdentifierModel.Username);
            parameters.Add(nameof(ProductIdentifierModel.PublicProductId), productIdentifierModel.PublicProductId);
            parameters.Add(nameof(UpdateProductDetailsModel.Name), updateProductDetailsModel.Name);
            parameters.Add(nameof(UpdateProductDetailsModel.Description), updateProductDetailsModel.Description);
            parameters.Add(nameof(UpdateProductDetailsModel.Quantity), updateProductDetailsModel.Quantity);

            var result = await conn.QuerySingleAsync<dynamic>("dbo.UpdateProduct", parameters, commandType: CommandType.StoredProcedure);
            
            if (result is null)
            {
                throw new DbUpdateException("Failed to get products");
            }

            return new Product
            {
                Id = result.PublicProductId,
                Name = result.ProductName,
                Description = result.Description,
                Quantity = result.Quantity,
                Price = result.Price,
                EnabledPrice = result.EnabledPrice,
                CurrencyCode = result.CurrencyCode
            };
        }
    }
}
