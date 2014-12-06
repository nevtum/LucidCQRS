using LucidCQRS.Common;

namespace LucidCQRS.EventStore
{
    public interface IEventFileSerializer
    {
        Event Deserialize(string filename);
        void Serialize(Event e, string filename);
    }
}
