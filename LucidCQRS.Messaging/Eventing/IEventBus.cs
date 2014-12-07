using System.Collections.Generic;
using LucidCQRS.Common;

namespace LucidCQRS.Messaging.Eventing
{
    public interface IEventBus
    {
        void Publish<T>(T domainEvent) where T : Event;
        void Publish<T>(IEnumerable<T> domainEvents) where T : Event;
    }
}
