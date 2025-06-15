namespace InventoryApi.Repository.Data.Webhook
{
    public record struct WebhookDetails(string WebhookURI, string SharedSecret, string TriggerAction);
}