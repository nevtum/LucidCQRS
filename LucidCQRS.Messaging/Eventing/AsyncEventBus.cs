using LucidCQRS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LucidCQRS.Messaging.Eventing
{
    public class AsyncEventBus : IEventBus, IEventSubscription
    {
        #region IEventBus

        public void Publish<T>(T domainEvent) where T : Event
        {
            throw new NotImplementedException();
        }

        public void Publish<T>(IEnumerable<T> domainEvents) where T : Event
        {
            Parallel.ForEach(domainEvents, e => Publish(e));
        }

        #endregion

        #region IEventSubscription

        public void Subscribe<T>(Action<T> action) where T : Event
        {
            throw new NotImplementedException();
        }

        public void ReleaseHandlers<T>() where T : Event
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
