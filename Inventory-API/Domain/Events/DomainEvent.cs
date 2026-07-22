namespace InventoryApi.Model.Events
{
    public abstract record DomainEvent(string streamName, int Version);
}
