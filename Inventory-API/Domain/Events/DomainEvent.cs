namespace Domain.Events
{
    public abstract record DomainEvent(string streamName, int Version);
}
