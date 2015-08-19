using System;
using System.Collections.Generic;
using LucidCQRS.Common;

namespace LucidCQRS.Messaging.Eventing
{
    /// <summary>
    /// A decorator class around another IEventBus to
    /// prevent recursive event publishing from occuring.
    /// This class will throw an exception if an event
    /// of the same type is concurrently been called
    /// more than once.
    /// </summary>
    public class ConcurrencyAwareEventBus : IEventBus
    {
        #region Fields

        private IEventBus _eventBus;
        private IList<string> _publishing;

        #endregion

        #region Constructors

        public ConcurrencyAwareEventBus(IEventBus eventBus)
        {
            _eventBus = eventBus;
            _publishing = new List<string>();
        }

        #endregion

        #region IEventBus

        public void Publish<T>(T domainEvent) where T : Event
        {
            Type t = domainEvent.GetType();

            if (_publishing.Contains(t.ToString()))
                throw new Exception("Cannot concurrently publish an event that is currently being published");

            _publishing.Add(t.ToString());
            _eventBus.Publish(domainEvent);
            _publishing.Remove(t.ToString());
        }

        #endregion
    }
}
