using LucidCQRS.Messaging.Exceptions;
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

        public void Send<T>(T command) where T : Command
        {
            Action<object> action;

            if (!handlers.TryGetValue(typeof(T), out action))
                throw new HandlerMissingException("Could not find handler for given command!");

            action.Invoke(command);
        }

        public void RegisterHandler<T>(Action<T> handler) where T : Command
        {
            Type commandType = typeof(T);

            if (handlers.ContainsKey(commandType))
                throw new DuplicateHandlerException("Cannot add more than one command handler!");

            handlers[commandType] = (c) =>
            {
                handler((T)c);
            };
        }
    }
}
