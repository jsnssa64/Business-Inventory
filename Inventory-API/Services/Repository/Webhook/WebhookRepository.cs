using System.Data;
using System.Data.Entity.Infrastructure;
using Dapper;
using KurrentDB.Client;
using Services.DataModel.Webhook;
using Shared.Constants;

namespace Services.Repository.Webhook
{
    public class WebhookRepository : IWebhookRepository
    {
        private IDbConnectionFactory _dbConnectionFactory;
        private KurrentDBClient _storeClient;

        private const string SubscribeUserToWebhookSP = "dbo.SubscribeUserToWebhook";
        private const string GetWebhooksByActionSP = "dbo.GetWebhooksByAction";

        public WebhookRepository(
                IDbConnectionFactory dbConnectionFactory,
                KurrentDBClient kurrentDBClient
            )
        {
            _storeClient = kurrentDBClient;
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task Subscribe(Subscription subscriptionModel)
        {
            using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add(nameof(Subscription.userName), subscriptionModel.userName);
            parameters.Add(nameof(Subscription.webhookUrl), subscriptionModel.webhookUrl);
            parameters.Add(nameof(Subscription.secret), subscriptionModel.secret);
            parameters.Add(nameof(Subscription.subscriptionType), subscriptionModel.subscriptionType);

            var result = await conn.ExecuteAsync(SubscribeUserToWebhookSP, parameters, commandType: CommandType.StoredProcedure);

            if(result != 1)
            {
                throw new DbUpdateException($"Failed to subscribe webhook for user: {subscriptionModel.userName}");
            }
        }

        public async Task<IEnumerable<WebhookDetails>> GetWebhookByAction(SubscriptionType subscriptionType)
        {
            using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add(nameof(SubscriptionType), subscriptionType);

            var result = await conn.QueryAsync<WebhookDetails>(GetWebhooksByActionSP, parameters, commandType: CommandType.StoredProcedure);

            //if(result is null)
            //    throw new DbUpdateException($"No webhook found for user: {getWebhookByAction.userName} with action: {getWebhookByAction.subscriptionType}");

            return result;
        }
    }
}
