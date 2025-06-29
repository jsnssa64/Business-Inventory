using KurrentDB.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services.Repository.EventStoreDbRepository
{
    public class EventStoreDbRepository: IEventStoreDbRepository
    {
        private KurrentDBClient _storeClient;

        public EventStoreDbRepository(KurrentDBClient storeClient)
        {
            _storeClient = storeClient;
        }

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
                    new EventData(
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
