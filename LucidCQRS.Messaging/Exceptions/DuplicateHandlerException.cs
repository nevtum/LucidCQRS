using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LucidCQRS.Messaging.Exceptions
{
    public class DuplicateHandlerException : Exception
    {
        public DuplicateHandlerException(string message)
            : base(message)
        {
        }
    }
}
