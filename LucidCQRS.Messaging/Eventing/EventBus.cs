using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LucidCQRS.Messaging.Eventing
{
    public class EventBus : IEventBus, IEventSubscription
    {
        #region IEventBus

        public void Publish<T>(T domainEvent) where T : Common.Event
        {
            throw new NotImplementedException();
        }

        public void Publish<T>(IEnumerable<T> domainEvents) where T : Common.Event
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEventSubscription

        void IEventSubscription.Subscribe<T>(Action<T> action)
        {
            throw new NotImplementedException();
        }

        void IEventSubscription.Unsubscribe<T>(Action<T> action)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
