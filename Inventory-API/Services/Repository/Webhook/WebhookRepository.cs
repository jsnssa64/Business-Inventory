using System.Data;
using System.Data.Entity.Infrastructure;
using Dapper;
using Services.DataModel.Webhook;

namespace Services.Repository.Webhook
{
    public class WebhookRepository(
        IDbConnectionFactory dbConnectionFactory) : IWebhookRepository
    {
        public async Task Subscribe(Subscription subscriptionModel)
        {
            using IDbConnection conn = dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

            DynamicParameters parameters = new();
            parameters.Add(nameof(Subscription.userName), subscriptionModel.userName);
            parameters.Add(nameof(Subscription.webhookUrl), subscriptionModel.webhookUrl);
            parameters.Add(nameof(Subscription.secret), subscriptionModel.secret);
            parameters.Add(nameof(Subscription.subscriptionType), subscriptionModel.subscriptionType);

            var result = await conn.ExecuteAsync("dbo.SubscribeUserToWebhook", parameters, commandType: CommandType.StoredProcedure);

            if(result != 1)
            {
                throw new DbUpdateException($"Failed to subscribe webhook for user: {subscriptionModel.userName}");
            }
        }


        public async Task<IEnumerable<WebhookDetails>> GetWebhookByAction(SubscriptionType subscriptionType)
        {
            using IDbConnection conn = dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

            DynamicParameters parameters = new();
            parameters.Add(nameof(SubscriptionType), subscriptionType);

            var result = await conn.QueryAsync<WebhookDetails>("dbo.GetWebhooksByAction", parameters, commandType: CommandType.StoredProcedure);

            return result;
        }
    }
}
