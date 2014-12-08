using System;
using LucidCQRS.Common;

namespace LucidCQRS.Messaging.Eventing
{
    public interface IEventSubscription
    {
        void Subscribe<T>(Action<T> action) where T: Event;
        void ReleaseHandlers<T>() where T : Event;
    }
}
