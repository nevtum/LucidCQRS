using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LucidCQRS.Messaging.Commanding
{
    public interface ICommandSubscription
    {
        void RegisterHandler<T>(Action<T> handler) where T : Command;
    }
}
