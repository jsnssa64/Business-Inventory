namespace InventoryApi.Repository.Data.Webhook
{
    public record struct ActionDetails(string action, DateTime actionTriggerTime);
}