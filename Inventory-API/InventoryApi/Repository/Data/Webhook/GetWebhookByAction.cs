namespace InventoryApi.Repository.Data.Webhook
{
    public record struct GetWebHookByAction(string userName, SubscriptionType subscriptionType);
}
