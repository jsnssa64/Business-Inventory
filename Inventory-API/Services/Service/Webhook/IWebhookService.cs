namespace Services.Service.Webhook
{
    public interface IWebhookService
    {
        Task PostWebhookBySubscription(string userName, SubscriptionType subscriptionType);
        Task PostToWebhook(IEnumerable<WebhookPost> webhooks, SubscriptionType subscriptionType);
    }
}
