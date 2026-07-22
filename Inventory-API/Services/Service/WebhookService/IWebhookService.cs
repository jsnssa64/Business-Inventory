using Services.DataModel.Webhook;

namespace Services.Service.UserService
{
    public interface IWebhookService
    {
        Task PostWebhookBySubscription(string userName, SubscriptionType subscriptionType);
        Task PostToWebhook(IEnumerable<WebhookPost> webhooks, SubscriptionType subscriptionType);
    }
}