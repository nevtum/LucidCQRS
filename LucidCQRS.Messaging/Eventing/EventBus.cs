using System;
using System.Collections.Generic;
using LucidCQRS.Common;

namespace LucidCQRS.Messaging.Eventing
{
    public class EventBus : IEventBus, IEventSubscription
    {
        #region Fields

        private IDictionary<Type, EventSubscribers> _handlers;

        #endregion

        #region Constructor

        public EventBus()
        {
            _handlers = new Dictionary<Type, EventSubscribers>();
        }

        #endregion

        #region IEventBus

        public void Publish<T>(T domainEvent) where T : Event
        {
            Type t = domainEvent.GetType();
            EventSubscribers subscribers;

            if (!_handlers.TryGetValue(domainEvent.GetType(), out subscribers))
                return;

            subscribers.Invoke(domainEvent);
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
    }
}
