using LucidCQRS.Common;
using LucidCQRS.Messaging.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LucidCQRS.Messaging.Eventing
{
    public class AsyncEventBus : IEventBus, IEventSubscription
    {
        #region Fields

        private IDictionary<Type, EventSubscribers> handlers;

        #endregion

        #region Constructor

        public AsyncEventBus()
        {
            handlers = new Dictionary<Type, EventSubscribers>();
        }

        #endregion

        #region IEventBus

        public void Publish<T>(T domainEvent) where T : Event
        {
            EventSubscribers subscribers;

            if (!handlers.TryGetValue(typeof(T), out subscribers))
                return;

            subscribers.Invoke(domainEvent);
        }

        public void Publish<T>(IEnumerable<T> domainEvents) where T : Event
        {
            Parallel.ForEach(domainEvents, e => Publish(e));
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
