using InventoryApi.Repository.Data.Webhook;

namespace InventoryApi.Repository.Webhook
{

    public interface IWebhookRepository
    {
        Task<IEnumerable<WebhookDetails>> GetWebhookByAction(SubscriptionType subscriptionType);
        Task Subscribe(Subscription subscriptionModel);
    }
}