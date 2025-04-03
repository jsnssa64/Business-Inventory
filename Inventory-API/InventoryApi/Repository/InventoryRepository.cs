using System.Data;
using System.Data.Entity.Infrastructure;
using System.Text.Json;
using Dapper;
using Domain.Inventory;
using EventStore.Client;
using InventoryApi.Factory;
using InventoryApi.Repository.Data;
using InventoryApi.Repository.Model;

namespace InventoryApi.Repository
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

        public async Task<bool> AddItemToInventoryByName(InventoryItem inventoryItem, Inventory inventory)
        {
            try
            {
                using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());
                
                var parameters = new { Name = inventoryItem.Name, Quantity = inventory.Quantity };
                var result = conn.QuerySingleOrDefault<int>("dbo.AddItemToInventoryByName", parameters, commandType: CommandType.StoredProcedure);

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

        public async Task<bool> AddItemToInventoryById(Inventory inventory)
        {
            using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add(nameof(AddItemToInventoryModel.ItemId), inventory.ItemId);
            parameters.Add(nameof(AddItemToInventoryModel.Quantity), inventory.Quantity);
            var result = await conn.ExecuteAsync("dbo.AddItemToInventoryById", parameters, commandType: CommandType.StoredProcedure);

            if (result <= 0)
            {
                throw new DbUpdateException("Failed to add item to inventory");
            }

            return true;
        }

        public async Task<InventoryItem> AddInventoryItem(InventoryItem inventoryItem)
        {
            using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add(nameof(AddInventoryItemModel.Name), inventoryItem.Name);
            parameters.Add(nameof(AddInventoryItemModel.Description), inventoryItem.Price);
            parameters.Add(nameof(AddInventoryItemModel.Price), inventoryItem.Price);
            parameters.Add(nameof(AddInventoryItemModel.CurrencyCode), inventoryItem.CurrencyCode);
            parameters.Add(nameof(AddInventoryItemModel.Quantity), inventoryItem.Quantity);
            parameters.Add(nameof(AddInventoryItemModel.NewItemId), dbType: DbType.Int32, direction: ParameterDirection.Output);

            await conn.ExecuteAsync("dbo.AddInventoryItem", parameters, commandType: CommandType.StoredProcedure);

            inventoryItem.ItemId = parameters.Get<int>(nameof(AddInventoryItemModel.NewItemId));

            if(inventoryItem.ItemId <= 0)
            {
                throw new DbUpdateException("Failed to add inventory item");
            }

            return inventoryItem;
        }

        public async Task<IEnumerable<InventoryItem>> GetInventoryItemByItemId(InventoryItem inventoryItem)
        {
            using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add(nameof(GetInventoryByItemIdModel.ItemId), inventoryItem.ItemId);

            var result = await conn.QueryAsync<InventoryItem>("dbo.GetInventoryItemByItemId", parameters, commandType: CommandType.StoredProcedure);

            if (result == null || result.Count() == 0)
            {
                throw new DbUpdateException("Failed to get inventory item");
            }

            return result;
        }

        public async Task<IEnumerable<InventoryInfo>> GetInventoryInfoByItemName(InventoryItem inventoryItem)
        {
            using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add(nameof(GetInventoryInfoByItemNameModel.ItemName), inventoryItem.Name);

            var result = await conn.QueryAsync<InventoryInfo>("dbo.GetInventoryInfoByItemName", parameters, commandType: CommandType.StoredProcedure);

            if (result == null || result.Count() == 0)
            {
                throw new DbUpdateException($"Failed to get inventory info for Item {inventoryItem.Name}");
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
