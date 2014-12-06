using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using LucidCQRS.Common;

namespace LucidCQRS.EventStore
{
    public class EventBinarySerializer : IEventFileSerializer
    {
        public void Serialize(Event e, string filename)
        {
            Stream stream = File.Open(filename, FileMode.Create);
            BinaryFormatter bFormatter = new BinaryFormatter();
            bFormatter.Serialize(stream, e);
            stream.Close();
        }

        public Event Deserialize(string filename)
        {
            Stream stream = File.Open(filename, FileMode.Open);
            BinaryFormatter bFormatter = new BinaryFormatter();
            Event e = (Event)bFormatter.Deserialize(stream);
            stream.Close();
            return e;
        }
    }
}
