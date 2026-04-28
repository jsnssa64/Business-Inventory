using Services.DataModel.Webhook;

namespace Services.Service.UserService
{
    public interface IWebhookService
    {
        Task PostToWebhook(IEnumerable<WebhookPost> webhooks, SubscriptionType subscriptionType);
    }
}