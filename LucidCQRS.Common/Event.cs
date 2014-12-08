using System;

namespace LucidCQRS.Common
{
    [Serializable]
    public abstract class Event : Message
    {
        public int Version;

        public Event(Guid aggregateId)
            : base(aggregateId)
        {
        }
    }
}
