using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LucidCQRS.Tests.Accounting;
using LucidCQRS.Common;
using LucidCQRS.EventStore;

namespace LucidCQRS.Tests
{
    [TestClass]
    public class AggregateTests
    {
        [TestMethod]
        public void ShouldProvideListOfEventsForModifiedAggregate()
        {
            Account acc = new Account(Guid.NewGuid(), "Joe Blow");
            acc.Deposit(500.00);
            acc.Withdraw(225.95);

            Assert.AreEqual(3, acc.GetUncommittedChanges().Count());

            acc.MarkChangesAsCommitted();

            Assert.AreEqual(0, acc.GetUncommittedChanges().Count());
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ShouldThrowExceptionWhenBreakingInvariant()
        {
            Account acc = new Account(Guid.NewGuid(), "Joe Blow");
            acc.Deposit(500.00);
            acc.Withdraw(1000);
        }

        [TestMethod]
        public void ShouldRestoreStateFromEvents()
        {
            Guid id = Guid.NewGuid();

            List<Event> storedEvents = new List<Event>();
            storedEvents.Add(new AccountCreated(id, "Richy Rich"));
            storedEvents.Add(new DepositMade(id, 530000.00));
            storedEvents.Add(new DepositMade(id, 18200.00));
            storedEvents.Add(new WithdrawalMade(id, 540.00));
            storedEvents.Add(new DepositMade(id, 461199.99));

            Account acc = new Account();
            acc.LoadFromHistory(storedEvents);

            Assert.AreEqual("Richy Rich", acc.GetAccountHolder());
            Assert.AreEqual(1008859.99, acc.GetCurrentBalance());
        }

        [TestMethod]
        public void ShouldSaveAndRestoreStateFromEvents()
        {
            IEventStore eventStore = new InMemoryEventStore();

            Guid id = Guid.NewGuid();

            Account acc = new Account(id, "Richy Rich");
            acc.Deposit(530000.00);
            acc.Deposit(18200.00);
            acc.Withdraw(540.00);
            acc.Deposit(461199.99);

            eventStore.SaveChanges(id, -1, acc.GetUncommittedChanges());
            acc.MarkChangesAsCommitted();

            Account restored = new Account();
            restored.LoadFromHistory(eventStore.GetEventsFor(id));

            Assert.AreEqual(acc.GetAccountHolder(), restored.GetAccountHolder());
            Assert.AreEqual(acc.GetCurrentBalance(), restored.GetCurrentBalance());
        }
    }
}
