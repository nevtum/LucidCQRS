using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LucidCQRS.Messaging.Commanding
{
    public class CommandBus : ICommandBus, ICommandSubscription
    {
        private IDictionary<Type, Action<object>> handlers;

        public CommandBus()
        {
            handlers = new Dictionary<Type, Action<object>>();
        }

        public void Send<T>(T command) where T : ICommand
        {
            Action<object> action;

            if (!handlers.TryGetValue(command.GetType(), out action))
                throw new Exception("Could not find handler for given command!");

            action.Invoke(command);
        }

        public void RegisterHandler<T>(Action<T> handler) where T : ICommand
        {
            Type commandType = typeof(T);

            if (handlers.ContainsKey(commandType))
                throw new Exception("Cannot add more than one command handler!");

            handlers[commandType] = (c) =>
            {
                handler((T)c);
            };
        }
    }
}
