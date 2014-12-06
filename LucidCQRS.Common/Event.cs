using System;

namespace LucidCQRS.Common
{
    [Serializable]
    public class Event : Message
    {
        public readonly Guid AggregateId;
        public int Version;

        public Event(Guid aggregateId)
        {
            AggregateId = aggregateId;
        }
    }
}
