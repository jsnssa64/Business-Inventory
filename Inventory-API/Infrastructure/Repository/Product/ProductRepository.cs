using System.Data;
using Dapper;
using Domain.ValueObjects.Product;
using Microsoft.IdentityModel.Tokens;
using Services.DataModel.Product;
using Services.DataModel.User;
using Services.Interface.Product;
using Shared.Constants;

namespace Infrastructure.Repository.Product
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

        public async Task<ProductIdModel> AddProduct(IDbConnection dbConnection, UserIdentifierModel userIdentifierModel, ProductDetailsModel productDetailsModel, IDbTransaction? dbTransaction)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add(nameof(UserIdentifierModel.Username), userIdentifierModel.Username);
            parameters.Add(nameof(ProductDetailsModel.ProductName), productDetailsModel.ProductName);
            parameters.Add(nameof(ProductDetailsModel.Description), productDetailsModel.Description);
            parameters.Add(nameof(ProductDetailsModel.Quantity), productDetailsModel.Quantity);
            parameters.Add(nameof(ProductIdModel.PublicProductId), dbType: DbType.Guid, direction: ParameterDirection.Output);

            var result = await dbConnection.ExecuteScalarAsync<int>("dbo.AddProduct", parameters, commandType: CommandType.StoredProcedure, transaction: dbTransaction);

            if (result != 0)
                throw new Exception("Unable to add product");

            var productIdentifierModel = new ProductIdentity()
            {
                Username = userIdentifierModel.Username,
                PublicProductId = parameters.Get<Guid>(nameof(ProductIdModel.PublicProductId))
            };

            return new ProductIdModel() { 
                PublicProductId = productIdentifierModel.PublicProductId
            };
        }

        public async Task AddPriceToProduct(IDbConnection dbConnection, ProductIdentity productIdentifierModel, PriceModel price, IDbTransaction? dbTransaction)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add(nameof(ProductIdentity.Username), productIdentifierModel.Username);
            parameters.Add(nameof(ProductIdentity.PublicProductId), productIdentifierModel.PublicProductId);
            parameters.Add(nameof(PriceModel.Price), price.Price);
            parameters.Add(nameof(PriceModel.CurrencyCode), price.CurrencyCode);

            var result = await dbConnection.ExecuteScalarAsync<int>("dbo.AddProductPrice", parameters, commandType: CommandType.StoredProcedure, transaction: dbTransaction);

            if (result != 0)
                throw new Exception("Unable to add product price");
        }

        public async Task<Product> GetProductById(ProductIdentity productIdentifierModel)
        {
            using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add(nameof(ProductIdentity.Username), productIdentifierModel.Username);
            parameters.Add(nameof(ProductIdentity.PublicProductId), productIdentifierModel.PublicProductId);

            var resultProduct = await conn.QuerySingleAsync<dynamic>("dbo.GetProductById", parameters, commandType: CommandType.StoredProcedure);

            if (resultProduct is null)
                throw new DbUpdateException("Failed to get product");

            dynamic? resultPrice = null;

            if (resultProduct.EnabledPrice)
            {
                resultPrice = await conn.QuerySingleAsync<dynamic>("dbo.GetProductPriceById", parameters, commandType: CommandType.StoredProcedure);


                if (resultPrice is null)
                    throw new DbUpdateException("Failed to get price");
            }

            var product = new Product();
            //product.Map(resultProduct, resultPrice);

            return product;
        }

        public async Task RemoveProductById(ProductIdentity productIdentifierModel)
        {
            using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add(nameof(ProductIdentity.Username), productIdentifierModel.Username);
            parameters.Add(nameof(ProductIdentity.PublicProductId), productIdentifierModel.PublicProductId);

            var result = await conn.ExecuteScalarAsync<int>("dbo.RemoveProductById", parameters, commandType: CommandType.StoredProcedure);

            if (result != 0)
                throw new Exception("Unable to remove product");
        }

        public async Task RemoveProductPrice(ProductIdentity productIdentifierModel)
        {
            using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add(nameof(ProductIdentity.Username), productIdentifierModel.Username);
            parameters.Add(nameof(ProductIdentity.PublicProductId), productIdentifierModel.PublicProductId);

            var result = await conn.ExecuteScalarAsync<int>("dbo.RemoveProductPrice", parameters, commandType: CommandType.StoredProcedure);

            if (result != 0)
                throw new Exception("Unable to remove product price");
        }

        public async Task<IEnumerable<Product>> GetProducts(UserIdentifierModel userIdentifierModel)
        {
            using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add(nameof(UserIdentifierModel.Username), userIdentifierModel.Username);

            var result = await conn.QueryAsync<Product>("dbo.GetProducts", parameters, commandType: CommandType.StoredProcedure);

            if (result.IsNullOrEmpty())
            {
                throw new DbUpdateException("Failed to get products");
            }

            return result;
        }

        public async Task UpdateProductPrice(ProductIdentity productIdentifierModel, UpdatePriceModel updatePrice)
        {
            using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add(nameof(ProductIdentity.Username), productIdentifierModel.Username);
            parameters.Add(nameof(ProductIdentity.PublicProductId), productIdentifierModel.PublicProductId);
            parameters.Add(nameof(UpdatePriceModel.Price), updatePrice.Price);
            parameters.Add(nameof(UpdatePriceModel.CurrencyCode), updatePrice.CurrencyCode);

            var result = await conn.ExecuteAsync("dbo.UpdateProductPrice", parameters, commandType: CommandType.StoredProcedure);

            if (result != 0)
                throw new DbUpdateException("Failed to update product price");
        }

        public async Task UpdateProduct(ProductIdentity productIdentifierModel, UpdateProductDetailsModel updateProductDetailsModel)
        {
            using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add(nameof(ProductIdentity.Username), productIdentifierModel.Username);
            parameters.Add(nameof(ProductIdentity.PublicProductId), productIdentifierModel.PublicProductId);
            parameters.Add(nameof(UpdateProductDetailsModel.Name), updateProductDetailsModel.Name);
            parameters.Add(nameof(UpdateProductDetailsModel.Description), updateProductDetailsModel.Description);
            parameters.Add(nameof(UpdateProductDetailsModel.Quantity), updateProductDetailsModel.Quantity);
            parameters.Add(nameof(UpdateProductDetailsModel.EnabledPrice), updateProductDetailsModel.EnabledPrice);

            var result = await conn.ExecuteScalarAsync<int>("dbo.UpdateProduct", parameters, commandType: CommandType.StoredProcedure);

            if (result != 0)
                throw new Exception("Unable to update product");
        }
    }
}
