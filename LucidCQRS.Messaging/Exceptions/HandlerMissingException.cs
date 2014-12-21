using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LucidCQRS.Messaging.Exceptions
{
    public class HandlerMissingException : Exception
    {
        public HandlerMissingException(string message)
            : base(message)
        {
        }
    }
}
