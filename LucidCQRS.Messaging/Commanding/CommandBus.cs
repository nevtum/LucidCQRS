using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LucidCQRS.Messaging.Commanding
{
    public class CommandBus : ICommandBus, ICommandSubscription
    {
        public void Send<T>(T command) where T : ICommand
        {
            throw new NotImplementedException();
        }

        public void RegisterHandler<T>(T command) where T : ICommand
        {
            throw new NotImplementedException();
        }
    }
}
