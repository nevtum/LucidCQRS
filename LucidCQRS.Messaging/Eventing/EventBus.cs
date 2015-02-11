using System;
using System.Collections.Generic;
using LucidCQRS.Common;

namespace LucidCQRS.Messaging.Eventing
{
    public class EventBus : IEventBus, IEventSubscription
    {
        #region Fields

        private IDictionary<Type, EventSubscribers> _handlers;
        private IList<string> _publishing;

        #endregion

        #region Constructor

        public EventBus()
        {
            _handlers = new Dictionary<Type, EventSubscribers>();
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
            LockedPublish<T>(domainEvent);
            _publishing.Remove(t.ToString());
        }

        public void Publish<T>(IEnumerable<T> domainEvents) where T : Event
        {
            foreach (T domainEvent in domainEvents)
            {
                Publish(domainEvent);
            }
        }

        #endregion

        #region IEventSubscription

        public void Subscribe<T>(Action<T> action) where T : Event
        {
            Type eventType = typeof(T);

            EventSubscribers subscribers;
            if (!_handlers.TryGetValue(eventType, out subscribers))
            {
                subscribers = new EventSubscribers();
                _handlers[eventType] = subscribers;
            }

            subscribers.Register(action);
        }

        public void ReleaseHandlers<T>() where T : Event
        {
            if (!_handlers.ContainsKey(typeof(T)))
                return;

            _handlers.Remove(typeof(T));
        }

        #endregion

        #region Private Methods

        private void LockedPublish<T>(T domainEvent) where T : Event
        {
            EventSubscribers subscribers;

            if (!_handlers.TryGetValue(domainEvent.GetType(), out subscribers))
                return;

            subscribers.Invoke(domainEvent);
        }

        #endregion
    }
}
