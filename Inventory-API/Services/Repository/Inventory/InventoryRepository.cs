using System.Data;
using System.Data.Entity.Infrastructure;
using Dapper;
using Domain.Entities.Inventory;
using KurrentDB.Client;
using Services.DataModel.Inventory;
using Services.DataModel.Product;
using Services.DataModel.User;
using Shared.Constants;

namespace InventoryApi.Repository.Inventory
{
    public class InventoryRepository : IInventoryRepository
    {
        private IDbConnectionFactory _dbConnectionFactory;
        private KurrentDBClient _storeClient;

        public InventoryRepository(
            IDbConnectionFactory dbConnectionFactory
            , KurrentDBClient storeClient
            )
        {
            _dbConnectionFactory = dbConnectionFactory;
            _storeClient = storeClient;
        }

        public async Task UpdateItemToInventoryByProductIdTransact(IDbConnection dbConnection,  ProductIdentifierModel productIdentifier, InventoryItemModel inventoryItemModel, IDbTransaction? dbTransaction)
        {
            try
            {
                await UpdateItemInInventory(dbConnection, productIdentifier, inventoryItemModel, dbTransaction);
            }
            catch(Exception ex)
            {
                throw new DbUpdateException($"Failed to add item to inventory: {ex.Message}");
            }            
        }

        public async Task UpdateItemToInventoryByProductId(ProductIdentifierModel productIdentifier, InventoryItemModel inventoryItemModel)
        {
            try
            {
                using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());
                await UpdateItemInInventory(conn, productIdentifier, inventoryItemModel);
            }
            catch (Exception ex)
            {
                throw new DbUpdateException($"Failed to add item to inventory: {ex.Message}");
            }
        }

        public async Task UpdateItemInInventory(IDbConnection dbConnection, ProductIdentifierModel productIdentifier, InventoryItemModel inventoryItemModel, IDbTransaction? dbTransaction = null)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add(nameof(ProductIdentifierModel.Username), productIdentifier.Username);
            parameters.Add(nameof(ProductIdentifierModel.PublicProductId), productIdentifier.PublicProductId);
            parameters.Add(nameof(InventoryItemModel.Quantity), inventoryItemModel.Quantity);

            var result = await dbConnection.ExecuteScalarAsync<int>("dbo.UpdateItemInInventory", parameters, commandType: CommandType.StoredProcedure, transaction: dbTransaction);

            if (result != 0)
            {
                throw new DbUpdateException($"Failed to Insert item");
            }
        }

        public async Task<InventoryItem> GetInventoryItemByProductId(ProductIdentifierModel productIdentifierModel)
        {
            using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add(nameof(ProductIdentifierModel.PublicProductId), productIdentifierModel.PublicProductId);
            parameters.Add(nameof(ProductIdentifierModel.Username), productIdentifierModel.Username);

            var result = await conn.QuerySingleAsync<InventoryItem>("dbo.GetInventoryByProductId", parameters, commandType: CommandType.StoredProcedure);

            if (result is null)
            {
                throw new DbUpdateException($"Failed to get inventory item: {productIdentifierModel.PublicProductId}");
            }

            return result;
        }

        public async Task<IEnumerable<InventoryItem>> GetInventoryItems(UserIdentifierModel userIdentifierModel)
        {
            using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add(nameof(UserIdentifierModel.Username), userIdentifierModel.Username);

            var result = await conn.QueryAsync<InventoryItem>("dbo.GetInventoryItems", parameters, commandType: CommandType.StoredProcedure);

            return result;
        }

        public Task<IWriteResult> AppendEventStream(object eventObject, string type, long intialPos)
        {
            throw new NotImplementedException();
        }

        public Task<List<ResolvedEvent>> ReadEventStream(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
