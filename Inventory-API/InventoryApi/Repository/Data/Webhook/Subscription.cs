namespace InventoryApi.Repository.Data.Webhook
{
    public record struct Subscription(string userName, Uri webhookUrl, SubscriptionType subscriptionType, string secret);
}
