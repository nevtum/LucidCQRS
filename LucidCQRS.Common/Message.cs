using System;

namespace LucidCQRS.Common
{
    [Serializable]
    public abstract class Message
    {
        public readonly Guid AggregateId;

        public Message(Guid aggregateId)
        {
            AggregateId = aggregateId;
        }
    }
}
