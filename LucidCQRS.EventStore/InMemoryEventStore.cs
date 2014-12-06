using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LucidCQRS.Common;

namespace LucidCQRS.EventStore
{
    public class InMemoryEventStore : IEventStore
    {
        #region Fields

        private readonly Dictionary<Guid, EventHistory> _store;

        #endregion

        #region Constructor

        public InMemoryEventStore()
        {
            _store = new Dictionary<Guid, EventHistory>();
        }

        #endregion

        #region IEventStore

        public void SaveChanges(Guid AggregateId, int expectedVersion, IEnumerable<Event> events)
        {
            EventHistory storedEvents;
            if (!_store.TryGetValue(AggregateId, out storedEvents))
            {
                storedEvents = new EventHistory();
                _store[AggregateId] = storedEvents;
            }

            storedEvents.Audit(expectedVersion);

            foreach (Event @e in events)
                storedEvents.SaveNew(AggregateId, @e);
        }

        public IEnumerable<Event> GetEventsFor(Guid AggregateId, int startVersion)
        {
            EventHistory storedEvents;
            if (!_store.TryGetValue(AggregateId, out storedEvents))
                throw new Exception("Could not find events for given aggregate!");

            return storedEvents.GetSavedSince(startVersion);
        }

        #endregion

        #region Private Classes

        internal class EventHistory
        {
            private List<Event> _saved = new List<Event>();
            private int _latestVersion = -1;

            public void SaveNew(Guid AggregateId, Event e)
            {
                System.Diagnostics.Debug.Assert(e.AggregateId == AggregateId);
                _latestVersion++;
                e.Version = _latestVersion;
                _saved.Add(e);
            }

            public IEnumerable<Event> GetSavedSince(int startVersion)
            {
                return _saved.Where(e => e.Version >= startVersion);
            }

            public void Audit(int expectedVersion)
            {
                if (_latestVersion != expectedVersion && expectedVersion != -1)
                    throw new Exception("Concurrency exception. Version audit fail!");
            }
        }

        #endregion
    }
}
