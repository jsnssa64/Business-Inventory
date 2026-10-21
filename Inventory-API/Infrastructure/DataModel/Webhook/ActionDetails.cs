namespace Infrastructure.DataModel.Webhook
{
    public record struct ActionDetails(string action, DateTime actionTriggerTime);
}