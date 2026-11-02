namespace Infrastructure.DataModel.Webhook
{
    public record struct Subscription(string UserId, Uri WebhookUrl, SubscriptionType SubscriptionType, string Secret);
}
