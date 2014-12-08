using LucidCQRS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

            if (!handlers.TryGetValue(typeof(T), out subscribers))
                throw new Exception("Could not find handlers for given event!");

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

        public void Unsubscribe<T>(Action<T> action) where T : Event
        {
            Type eventType = typeof(T);

            EventSubscribers subscribers;
            if (!handlers.TryGetValue(eventType, out subscribers))
                throw new Exception("Subscription does not exist!");

            subscribers.Unregister(action);
        }

        #endregion
    }
}
