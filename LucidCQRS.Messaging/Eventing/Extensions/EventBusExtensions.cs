using System.Collections.Generic;
using LucidCQRS.Common;
using System.Threading.Tasks;

namespace LucidCQRS.Messaging.Eventing.Extensions
{
    public static class EventBusExtensions
    {
        public static void PublishBatch<T>(this IEventBus eventBus, IEnumerable<T> domainEvents) where T : Event
        {
            foreach (T domainEvent in domainEvents)
            {
                eventBus.Publish<T>(domainEvent);
            }
        }

        // Warning: has not been well tested.
        public static void PublishBatchAsync<T>(this IEventBus eventBus, IEnumerable<T> domainEvents) where T : Event
        {
            Parallel.ForEach(domainEvents, e => eventBus.Publish<T>(e));
        }
    }
}
