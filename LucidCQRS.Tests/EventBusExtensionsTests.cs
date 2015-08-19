using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LucidCQRS.Messaging.Eventing;
using LucidCQRS.Messaging.Eventing.Extensions;
using LucidCQRS.Tests.Accounting;
using LucidCQRS.Common;
using System.Collections.Generic;

namespace LucidCQRS.Tests
{
    [TestClass]
    public class EventBusExtensionsTests
    {
        [TestMethod]
        public void ShouldPublishBatchEvents()
        {
            Guid guid = Guid.NewGuid();
            double amount = 0.0;

            EventBus eventBus = new EventBus();

            Action<DepositMade> Handle = (e) =>
            {
                amount += e.Amount;
            };

            Action<WithdrawalMade> Handle2 = (e) =>
            {
                amount -= e.Amount;
            };

            eventBus.Subscribe<DepositMade>(Handle);
            eventBus.Subscribe<WithdrawalMade>(Handle2);

            List<Event> events = new List<Event>();
            events.Add(new DepositMade(guid, 73));
            events.Add(new WithdrawalMade(guid, 52));
            events.Add(new DepositMade(guid, 48));
            events.Add(new WithdrawalMade(guid, 24));

            eventBus.PublishBatch(events);

            Assert.AreEqual(45.00, amount);
        }
    }
}
