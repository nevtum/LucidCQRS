using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LucidCQRS.Common;

namespace LucidCQRS.EventStore
{
    public class FileBasedEventStore : IEventStore
    {
        #region Fields

        private IEventFileSerializer _serializer;
        private string _rootDir;

        #endregion

        #region Constructor

        public FileBasedEventStore(string rootDir)
        {
            _serializer = new EventBinarySerializer();
            _rootDir = rootDir;
        }

        #endregion

        #region IEventStore

        public void SaveChanges(Guid AggregateId, int expectedVersion, IEnumerable<Event> events)
        {
            string aggregatePath = string.Format("{0}\\{1}", _rootDir, AggregateId);

            IEnumerable<Event> storedEvents = TryGetEventsFromFile(AggregateId, aggregatePath, 0);

            if (storedEvents.Count() == 0)
                Directory.CreateDirectory(aggregatePath);

            else
                if (storedEvents.Last().Version != expectedVersion && expectedVersion != -1)
                    throw new Exception("Concurrency exception. Version audit fail!");

            int latestVersion = expectedVersion;
            foreach (Event @e in events)
            {
                latestVersion++;
                @e.Version = latestVersion;
                _serializer.Serialize(@e, string.Format("{0}\\{1}.es", aggregatePath, @e.Version));
            }
        }

        public IEnumerable<Event> GetEventsFor(Guid AggregateId, int startVersion)
        {
            string aggregatePath = string.Format("{0}\\{1}", _rootDir, AggregateId);

            if (!Directory.Exists(aggregatePath))
                throw new Exception("Could not find events for given aggregate!");

            return TryGetEventsFromFile(AggregateId, aggregatePath, startVersion);
        }

        #endregion

        #region Private Methods

        private IEnumerable<Event> TryGetEventsFromFile(Guid AggregateId, string directory, int startVersion)
        {
            if (!Directory.Exists(directory))
                return new List<Event>();

            return Directory.EnumerateFiles(directory, "*.es")
                .Where(filename => VersionLargerThan(filename, startVersion))
                .Select(file => _serializer.Deserialize(file));
        }

        private bool VersionLargerThan(string filename, int startVersion)
        {
            string version = Path.GetFileNameWithoutExtension(filename);
            return int.Parse(version) >= startVersion;
        }

        #endregion
    }
}
