using System.Data;
using System.Data.Entity.Infrastructure;
using System.Text.Json;
using Dapper;
using Domain.Inventory;
using EventStore.Client;
using InventoryApi.Factory;
using InventoryApi.Repository.Data;
using InventoryApi.Repository.Model;
using Microsoft.IdentityModel.Tokens;

namespace InventoryApi.Repository.Inventory
{
    public class InventoryRepository: IInventoryRepository
    {
        private IDbConnectionFactory _dbConnectionFactory;
        private EventStoreClient _storeClient;

        public InventoryRepository(
            EventStoreClient storeClient, 
            IDbConnectionFactory dbConnectionFactory
            )
        {
            _dbConnectionFactory = dbConnectionFactory;
            _storeClient = storeClient;
        }

        public async Task<bool> AddItemToInventoryByName(Product product, InventoryItem inventoryItem)
        {
            try
            {
                using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());
                
                var parameters = new { product.Name, inventoryItem.InventoryQuantity };
                var result = await conn.QuerySingleOrDefaultAsync<int>("dbo.AddItemToInventoryByName", parameters, commandType: CommandType.StoredProcedure);

                if (result == -1)
                {
                    throw new DbUpdateException($"Failed to Insert item");
                }
                else if(result == -2)
                {
                    throw new DbUpdateException($"Failed to Insert item");
                }

                return true;
            }
            catch(Exception ex)
            {
                throw new DbUpdateException($"Failed to add item to inventory: {ex.Message}");
            }
            
        }

        public async Task<bool> AddItemToInventoryByProductId(InventoryItem inventoryItem)
        {
            using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add(nameof(AddItemToInventoryByProductIdModel.ProductId), inventoryItem.ProductId);
            parameters.Add(nameof(AddItemToInventoryByProductIdModel.Quantity), inventoryItem.InventoryQuantity);
            var result = await conn.ExecuteAsync("dbo.AddItemToInventoryByProductId", parameters, commandType: CommandType.StoredProcedure);

            if (result <= 0)
            {
                throw new DbUpdateException("Failed to add item to inventory");
            }

            return true;
        }

        public async Task<Product> AddProduct(Product product)
        {
            using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add(nameof(AddInventoryItemModel.Name), product.Name);
            parameters.Add(nameof(AddInventoryItemModel.Description), product.Price);
            parameters.Add(nameof(AddInventoryItemModel.Price), product.Price);
            parameters.Add(nameof(AddInventoryItemModel.CurrencyCode), product.CurrencyCode);
            parameters.Add(nameof(AddInventoryItemModel.Quantity), product.Quantity);
            parameters.Add(nameof(AddInventoryItemModel.NewProductId), dbType: DbType.Int32, direction: ParameterDirection.Output);

            await conn.ExecuteAsync("dbo.AddProduct", parameters, commandType: CommandType.StoredProcedure);

            product.Id = parameters.Get<int>(nameof(AddInventoryItemModel.NewProductId));

            if(product.Id <= 0)
            {
                throw new DbUpdateException("Failed to add inventory item");
            }

            return product;
        }

        public async Task<IEnumerable<Product>> GetProductById(Product product)
        {
            using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add(nameof(GetProductByIdModel.ProductId), product.Id);

            var result = await conn.QueryAsync<Product>("dbo.GetProductById", parameters, commandType: CommandType.StoredProcedure);

            if (result.IsNullOrEmpty())
            {
                throw new DbUpdateException("Failed to get product");
            }

            return result;
        }

        public async Task<IEnumerable<InventoryInfo>> GetInventoryInfoByItemName(Product product)
        {
            using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add(nameof(GetInventoryByProductNameModel.ProductName), product.Name);

            var result = await conn.QueryAsync<InventoryInfo>("dbo.GetInventoryByProductName", parameters, commandType: CommandType.StoredProcedure);

            if (result.IsNullOrEmpty())
            {
                throw new DbUpdateException($"Failed to get inventory info for Item {product.Name}");
            }

            return result;
        }

        public async Task<List<ResolvedEvent>> ReadEventStream(CancellationToken cancellationToken)
        {
            var events = _storeClient.ReadAllAsync(Direction.Forwards, Position.Start, 1, false);

            return await events.ToListAsync(cancellationToken);
        }

        public async Task<IWriteResult> AppendEventStream(object eventObject, string type, long intialPos)
        {
            var streamName = eventObject.GetType().Name;
            var stream = $"{type}-{streamName}";
            var events = new List<EventData>
                {
                    new EventData(
                        Uuid.NewUuid(),
                        eventObject.GetType().Name,
                        JsonSerializer.SerializeToUtf8Bytes(eventObject)
                    )
                };
            return await _storeClient.AppendToStreamAsync(stream, StreamState.Any, events);
        }
    }
}
