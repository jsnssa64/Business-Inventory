using KurrentDB.Client;
using System.Text.Json;
using Services.Interface;

namespace Infrastructure.Repository.EventStoreDbRepository
{
    public class EventStoreDbRepository(KurrentDBClient storeClient) : IEventStoreDbRepository
    {
        private readonly KurrentDBClient _storeClient = storeClient;

        public async Task<List<ResolvedEvent>> ReadEventStream(object eventObject, string type, CancellationToken cancellationToken)
        {
            var streamName = eventObject.GetType().Name;
            var stream = GetStreamName(type, streamName);

            var result = _storeClient.ReadStreamAsync(
                Direction.Forwards,
                stream,
                StreamPosition.Start,
                cancellationToken: cancellationToken
            );

            return await result.ToListAsync(cancellationToken);
        }

        public async Task<IWriteResult> AppendEventStream(object eventObject, string type, long intialPos)
        {
            var streamName = eventObject.GetType().Name;
            var stream = GetStreamName(type, streamName);
            var events = new List<EventData>
                {
                    new(
                        Uuid.NewUuid(),
                        eventObject.GetType().Name,
                        JsonSerializer.SerializeToUtf8Bytes(eventObject)
                    )
                };
            return await _storeClient.AppendToStreamAsync(stream, StreamState.Any, events);
        }

        private string GetStreamName(string type, string streamName)
        {
            return $"{type}-{streamName}";
        }
    }
}
