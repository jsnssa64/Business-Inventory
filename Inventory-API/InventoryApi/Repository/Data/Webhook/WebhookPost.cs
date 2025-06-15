namespace InventoryApi.Repository.Data.Webhook
{
    public record struct WebhookPost(string WebhookURI, string SharedSecret, ActionDetails payload);
}