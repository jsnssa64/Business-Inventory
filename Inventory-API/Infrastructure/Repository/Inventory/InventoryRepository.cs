using System.Data;
using Dapper;
using Domain.Entities.Inventory;
using Domain.ValueObjects.Product;
using KurrentDB.Client;
using Services.DataModel.Inventory;
using Services.DataModel.Product;
using Services.DataModel.User;
using Shared.Constants;

namespace Infrastructure.Repository.Inventory
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

        public async Task UpdateItemToInventoryByProductIdTransact(IDbConnection dbConnection,  ProductIdentity productIdentifier, InventoryItemModel inventoryItemModel, IDbTransaction? dbTransaction)
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

        public async Task UpdateItemToInventoryByProductId(ProductIdentity productIdentifier, InventoryItemModel inventoryItemModel)
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

        public async Task UpdateItemInInventory(IDbConnection dbConnection, ProductIdentity productIdentifier, InventoryItemModel inventoryItemModel, IDbTransaction? dbTransaction = null)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add(nameof(ProductIdentity.Username), productIdentifier.Username);
            parameters.Add(nameof(ProductIdentity.PublicProductId), productIdentifier.PublicProductId);
            parameters.Add(nameof(InventoryItemModel.Quantity), inventoryItemModel.Quantity);

            var result = await dbConnection.ExecuteScalarAsync<int>("dbo.UpdateItemInInventory", parameters, commandType: CommandType.StoredProcedure, transaction: dbTransaction);

            if (result != 0)
            {
                throw new DbUpdateException($"Failed to Insert item");
            }
        }

        public async Task<InventoryItem> GetInventoryItemByProductId(ProductIdentity productIdentifierModel)
        {
            using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add(nameof(ProductIdentity.PublicProductId), productIdentifierModel.PublicProductId);
            parameters.Add(nameof(ProductIdentity.Username), productIdentifierModel.Username);

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
