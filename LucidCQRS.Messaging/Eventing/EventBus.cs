using System;
using System.Collections.Generic;
using LucidCQRS.Common;

namespace LucidCQRS.Messaging.Eventing
{
    public class EventBus : IEventBus, IEventSubscription
    {
        #region Fields

        private IDictionary<Type, EventSubscribers> handlers;

        #endregion

        #region Constructor

        public EventBus()
        {
            handlers = new Dictionary<Type, EventSubscribers>();
        }

        #endregion

        #region IEventBus

        public void Publish<T>(T domainEvent) where T : Event
        {
            EventSubscribers subscribers;

            if (!handlers.TryGetValue(domainEvent.GetType(), out subscribers))
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
            if (!handlers.TryGetValue(eventType, out subscribers))
            {
                subscribers = new EventSubscribers();
                handlers[eventType] = subscribers;
            }

            subscribers.Register(action);
        }

        public void ReleaseHandlers<T>() where T : Event
        {
            if (!handlers.ContainsKey(typeof(T)))
                return;

            handlers.Remove(typeof(T));
        }

        #endregion
    }
}
