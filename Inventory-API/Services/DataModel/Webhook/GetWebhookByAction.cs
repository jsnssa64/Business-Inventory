namespace Services.DataModel.Webhook
{
    public record struct GetWebHookByAction(string userName, SubscriptionType subscriptionType);
}
