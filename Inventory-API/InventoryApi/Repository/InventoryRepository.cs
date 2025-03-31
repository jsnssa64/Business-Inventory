using System.Text.Json;
using System.Threading;
using EventStore.Client;

namespace InventoryApi.Repository
{
    public class InventoryRepository: IInventoryRepository
    {
        private EventStoreClient _storeClient;

        public InventoryRepository(EventStoreClient storeClient)
        {

            _storeClient = storeClient;
        }

        public async Task<List<ResolvedEvent>> ReadEventStream(CancellationToken cancellationToken)
        {
            var events = _storeClient.ReadAllAsync(Direction.Forwards, Position.Start, 1, false);

            return await events.ToListAsync(cancellationToken);
        }

        public async Task<IWriteResult> AppendEventStream(object eventObject, string type, long intialPos)
        {
            var streamName = eventObject.GetType().Name;
            var stream = $"{type}-{streamName}";
            var events = new List<EventData>
                {
                    new EventData(
                        Uuid.NewUuid(),
                        eventObject.GetType().Name,
                        JsonSerializer.SerializeToUtf8Bytes(eventObject)
                    )
                };
            return await _storeClient.AppendToStreamAsync(stream, StreamState.Any, events);
        }
    }
}
