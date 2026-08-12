namespace Services.DataModel.Webhook
{
    public record struct ActionDetails(string action, DateTime actionTriggerTime);
}