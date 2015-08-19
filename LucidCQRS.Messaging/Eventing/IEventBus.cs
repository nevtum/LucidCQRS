using System.Collections.Generic;
using LucidCQRS.Common;

namespace LucidCQRS.Messaging.Eventing
{
    public interface IEventBus
    {
        void Publish<T>(T domainEvent) where T : Event;
    }
}
