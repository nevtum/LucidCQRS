using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LucidCQRS.EventStore;
using LucidCQRS.Common;
using LucidCQRS.Tests.Accounting;
using System.IO;
using LucidCQRS.Messaging.Eventing;

namespace LucidCQRS.Tests
{
    [TestClass]
    public class EventStoreTests
    {
        private IEventStore _store;

        [TestInitialize]
        public void Setup()
        {
            string rootDir = "MyPath";
            ClearTestArtifacts(rootDir);
            _store = new FileBasedEventStore(rootDir);
            //_store = new InMemoryEventStore();
        }

        [TestMethod]
        public void ShouldSaveAndRetrieveFromFileBasedEventStore()
        {
            Guid id = Guid.NewGuid();

            List<Event> originalEvents = new List<Event>();
            originalEvents.Add(new DepositMade(id, 220.50));
            originalEvents.Add(new WithdrawalMade(id, 45.25));

            _store.SaveChanges(id, -1, originalEvents);

            List<Event> deserializedEvents = _store.GetEventsFor(id).ToList();

            for (int i = 0; i < deserializedEvents.Count; i++)
            {
                Assert.AreEqual(originalEvents[i].AggregateId, deserializedEvents[i].AggregateId);
                Assert.AreEqual(originalEvents[i].Version, deserializedEvents[i].Version);
            }
        }

        [TestMethod]
        public void ShouldPublishEventsPreviouslySaved()
        {
            EventBus eventBus = new EventBus();

            List<Type> eventsTriggered = new List<Type>();

            eventBus.Subscribe<DepositMade>((e) =>
            {
                eventsTriggered.Add(e.GetType());
            });

            eventBus.Subscribe<WithdrawalMade>((e) =>
            {
                eventsTriggered.Add(e.GetType());
            });

            Guid id = Guid.NewGuid();

            List<Event> originalEvents = new List<Event>();
            originalEvents.Add(new DepositMade(id, 220.50));
            originalEvents.Add(new WithdrawalMade(id, 45.25));

            _store.SaveChanges(id, -1, originalEvents);

            foreach (Event ev in _store.GetEventsFor(id))
                eventBus.Publish(ev);

            Assert.AreEqual(2, eventsTriggered.Count);
            Assert.AreEqual(true, eventsTriggered.Contains(typeof(DepositMade)));
            Assert.AreEqual(true, eventsTriggered.Contains(typeof(WithdrawalMade)));
            Assert.AreEqual(false, eventsTriggered.Contains(typeof(AccountCreated)));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ShouldThrowExceptionWhenEventsForAggregateNotFound()
        {
            Guid id = new Guid();
            _store.GetEventsFor(id);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ShouldThrowExceptionWhenIncorrectVersionOnSecondSave()
        {
            Guid id = Guid.NewGuid();

            List<Event> events1 = new List<Event>();
            events1.Add(new AccountCreated(id, "Mrs. Smith"));

            List<Event> events2 = new List<Event>();
            events2.Add(new DepositMade(id, 4200.50));

            _store.SaveChanges(id, -1, events1);
            _store.SaveChanges(id, 2, events2); // incorrect version provided
        }

        #region Helper Methods

        private void ClearTestArtifacts(string path)
        {
            DirectoryInfo root = new DirectoryInfo(path);

            if (root.Exists)
                root.Delete(true);
        }

        #endregion
    }
}
