namespace Infrastructure.DataModel.Webhook
{
    public record struct GetWebHookByAction(string Username, SubscriptionType SubscriptionType);
}
