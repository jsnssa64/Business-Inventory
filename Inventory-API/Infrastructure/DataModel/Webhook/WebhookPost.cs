namespace Infrastructure.DataModel.Webhook
{
    public record struct WebhookPost(string WebhookURI, string SharedSecret, ActionDetails payload);
}