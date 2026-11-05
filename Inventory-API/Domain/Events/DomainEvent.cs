namespace Domain.Events
{
    public abstract record DomainEvent(string StreamName, int Version);
}
