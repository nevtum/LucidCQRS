using System;
using System.Collections.Generic;
using LucidCQRS.Common;

namespace LucidCQRS.EventStore
{
    public interface IEventStore
    {
        void SaveChanges(Guid AggregateId, int expectedVersion, IEnumerable<Event> events);
        IEnumerable<Event> GetEventsFor(Guid AggregateId, int startVersion = 0);
    }
}
