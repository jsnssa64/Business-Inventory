using InventoryApi.Model.Events;
using System.Collections.Concurrent;

namespace InventoryApi.Model.Action
{

    public abstract class Aggregate
    {
        public Aggregate()
        {
            Uncommited = new ConcurrentDictionary<Version, DomainEvent>();
        }

        public record Version(int version);

        public const int _maximumUncommited = 50;

        private readonly Object lockObject = new();

        public ConcurrentDictionary<Version, DomainEvent> Uncommited;

        public void AddEvent(DomainEvent domainEvent)
        {
            if (domainEvent == null)
                throw new ArgumentNullException(nameof(domainEvent));

            if (Uncommited.Count >= _maximumUncommited)
                throw new InvalidOperationException($"Cannot add more than {_maximumUncommited} uncommitted events.");

            if(!Uncommited.TryAdd(new Version(domainEvent.Version), domainEvent))
            {
                Console.WriteLine("Version already exists in Uncommited");
                return;
            }

        }

        public void commit()
        {
            List<KeyValuePair<Version, DomainEvent>> orderedUncommitedVersion;

            //  Lock Uncommited so that between committing and clearing we dont delete anything accidentally
            lock (lockObject)
            {
                if (Uncommited.Count < _maximumUncommited)
                    return;

                orderedUncommitedVersion = [.. Uncommited.OrderBy(kv => kv.Key)];
                Uncommited.Clear();
            }

            // Send this to event store
            // orderedUncommitedVersion
        }
    }
}
