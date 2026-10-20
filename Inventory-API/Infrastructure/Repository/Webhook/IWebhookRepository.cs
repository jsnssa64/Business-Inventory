using Services.DataModel.Webhook;

namespace Services.Repository.Webhook
{

    public interface IWebhookRepository
    {
        Task<IEnumerable<WebhookDetails>> GetWebhooksByAction(SubscriptionType subscriptionType);
        Task Subscribe(Subscription subscriptionModel);
    }
}