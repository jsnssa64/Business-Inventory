namespace Services.DataModel.Webhook
{
    public record struct WebhookDetails(string WebhookURI, string SharedSecret, string TriggerAction);
}