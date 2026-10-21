namespace Infrastructure.DataModel.Webhook
{
    public record struct GetWebHookByAction(string userName, SubscriptionType subscriptionType);
}
