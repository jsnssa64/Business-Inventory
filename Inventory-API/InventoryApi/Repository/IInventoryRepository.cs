using EventStore.Client;

namespace InventoryApi.Repository
{
    public interface IInventoryRepository
    {
        Task<List<ResolvedEvent>> ReadEventStream(CancellationToken cancellationToken);
        Task<IWriteResult> AppendEventStream(object eventObject, string type, long intialPos);
    }
}