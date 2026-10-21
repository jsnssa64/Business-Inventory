using Dapper;
using Domain.Entities.Inventory;
using Infrastructure.DataModel.User;
using KurrentDB.Client;
using Services.Interface;
using Shared.Constants;
using System.Data;

namespace Infrastructure.Repository.Inventory
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

        public async Task<int> UpdateItemInInventory(ProductIdentifierModel productIdentifier, InventoryItemModel inventoryItemModel, IDbConnection dbConnection, IDbTransaction? dbTransaction, CancellationToken cancellationToken)
        {
            DynamicParameters parameters = new();
            parameters.Add(nameof(ProductIdentifierModel.Username), productIdentifier.Username);
            parameters.Add(nameof(ProductIdentifierModel.PublicProductId), productIdentifier.PublicProductId);
            parameters.Add(nameof(InventoryItemModel.Quantity), inventoryItemModel.Quantity);

            return result = await dbConnection.ExecuteScalarAsync<int>(new CommandDefinition(StoredProc.UpdateItemInInventory, parameters, transaction: dbTransaction, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken));
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
