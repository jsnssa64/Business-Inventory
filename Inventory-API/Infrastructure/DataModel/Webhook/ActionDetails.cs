namespace Infrastructure.DataModel.Webhook
{
    public record struct ActionDetails(string Action, DateTime ActionTriggerTime);
}