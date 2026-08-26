namespace Services.DataModel.Webhook
{
    public record struct WebhookPost(string WebhookURI, string SharedSecret, ActionDetails payload);
}