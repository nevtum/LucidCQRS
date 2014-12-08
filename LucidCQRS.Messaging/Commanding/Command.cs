using System;
using LucidCQRS.Common;

namespace LucidCQRS.Messaging.Commanding
{
    public abstract class Command : Message
    {
        public Command(Guid aggregateId)
            : base(aggregateId)
        {
        }
    }
}
