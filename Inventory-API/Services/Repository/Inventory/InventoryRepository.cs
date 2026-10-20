using System.Data;
using System.Data.Entity.Infrastructure;
using Dapper;
using Domain.Entities.Inventory;
using KurrentDB.Client;
using Services.DataModel.Inventory;
using Services.DataModel.Product;
using Services.DataModel.User;

namespace Services.Repository.Inventory
{
    public class InventoryRepository(
            IDbConnectionFactory dbConnectionFactory,
            KurrentDBClient storeClient) : IInventoryRepository
    {
        private static class StoredProc
        {
            public const string UpdateItemInInventory = "dbo.UpdateItemInInventory";
            public const string GetInventoryByProductId = "dbo.GetInventoryByProductId";
            public const string GetInventoryItems = "dbo.GetInventoryItems";
        }

        private readonly IDbConnectionFactory _dbConnectionFactory = dbConnectionFactory;
        private readonly KurrentDBClient _storeClient = storeClient;

        public async Task UpdateItemInInventory(ProductIdentifierModel productIdentifier, InventoryItemModel inventoryItemModel, CancellationToken cancellationToken)
        {
            using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb);
            await UpdateItemInInventory(productIdentifier, inventoryItemModel, conn, null, cancellationToken);
        }

        public async Task UpdateItemInInventory(ProductIdentifierModel productIdentifier, InventoryItemModel inventoryItemModel, IDbConnection dbConnection, IDbTransaction? dbTransaction, CancellationToken cancellationToken)
        {
            DynamicParameters parameters = new();
            parameters.Add(nameof(ProductIdentifierModel.Username), productIdentifier.Username);
            parameters.Add(nameof(ProductIdentifierModel.PublicProductId), productIdentifier.PublicProductId);
            parameters.Add(nameof(InventoryItemModel.Quantity), inventoryItemModel.Quantity);

            var result = await dbConnection.ExecuteScalarAsync<int>(new CommandDefinition(StoredProc.UpdateItemInInventory, parameters, transaction: dbTransaction, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken));

            if (result != 0)
                throw new DbUpdateException($"Failed to Insert item");
        }

        public async Task<InventoryItem?> GetInventoryItemByProductId(ProductIdentifierModel productIdentifierModel, CancellationToken cancellationToken)
        {
            using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

            DynamicParameters parameters = new();
            parameters.Add(nameof(ProductIdentifierModel.PublicProductId), productIdentifierModel.PublicProductId);
            parameters.Add(nameof(ProductIdentifierModel.Username), productIdentifierModel.Username);

            var result = await conn.QuerySingleOrDefaultAsync<InventoryItem>(new CommandDefinition(StoredProc.GetInventoryByProductId, parameters, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken));

            return result;
        }

        public async Task<IEnumerable<InventoryItem>> GetInventoryItems(UserIdentifierModel userIdentifierModel, CancellationToken cancellationToken)
        {
            using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

            DynamicParameters parameters = new();
            parameters.Add(nameof(UserIdentifierModel.Username), userIdentifierModel.Username);

            var result = await conn.QueryAsync<InventoryItem>(new CommandDefinition(StoredProc.GetInventoryItems, parameters, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken));

            return result;
        }
    }
}
