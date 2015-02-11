using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LucidCQRS.Messaging.Eventing;
using LucidCQRS.Tests.Accounting;
using LucidCQRS.Messaging.Exceptions;

namespace LucidCQRS.Tests
{
    [TestClass]
    public class EventBusTests
    {
        [TestMethod]
        public void ShouldTriggerSubscribedHandlerWhenEventIsPublished()
        {
            bool triggered = false;

            Action<AccountCreated> Handle = (e) =>
            {
                triggered = true;
            };

            EventBus eventBus = new EventBus();
            eventBus.Subscribe(Handle);
            eventBus.Publish(new AccountCreated(Guid.NewGuid(), "Test"));

            Assert.AreEqual(true, triggered);
        }

        [TestMethod]
        public void ShouldTriggerMultipleHandlersWhenEventIsPublished()
        {
            bool triggered1 = false;
            bool triggered2 = false;

            Action<AccountCreated> Handle1 = (e) =>
            {
                triggered1 = true;
            };

            Action<AccountCreated> Handle2 = (e) =>
            {
                triggered2 = true;
            };

            EventBus eventBus = new EventBus();
            eventBus.Subscribe(Handle1);
            eventBus.Subscribe(Handle2);
            eventBus.Publish(new AccountCreated(Guid.NewGuid(), "Test"));

            Assert.AreEqual(true, triggered1);
            Assert.AreEqual(true, triggered2);
        }

        [TestMethod]
        public void ShouldNotThrowExceptionWhenEventPublishedAfterHandlersReleased()
        {
            bool triggered = false;

            Action<AccountCreated> Handle = (e) =>
            {
                triggered = true;
            };

            EventBus eventBus = new EventBus();
            eventBus.Subscribe(Handle);
            eventBus.ReleaseHandlers<AccountCreated>();
            eventBus.Publish(new AccountCreated(Guid.NewGuid(), "Test"));

            Assert.AreEqual(false, triggered);
        }

        [TestMethod]
        public void ShouldNotThrowExceptionIfNoHandlersRegistered()
        {
            IEventBus eventBus = new EventBus();
            eventBus.Publish(new AccountCreated(Guid.NewGuid(), "Test"));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ShouldThrowExceptionForConcurrentPublishes()
        {
            EventBus eventBus = new EventBus();

            Action<DepositMade> Handle = (e) =>
            {
                // Calls publish again while handling same event.
                // Should be prevented by EventBus.
                eventBus.Publish<DepositMade>(new DepositMade(Guid.NewGuid(), 20));
            };

            eventBus.Subscribe<DepositMade>(Handle);

            eventBus.Publish<DepositMade>(new DepositMade(Guid.NewGuid(), 40));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ShouldPreventRoundTripConcurrentPublishes()
        {
            EventBus eventBus = new EventBus();

            Action<DepositMade> Handle = (e) =>
            {
                eventBus.Publish<WithdrawalMade>(new WithdrawalMade(Guid.NewGuid(), 20));
            };

            Action<WithdrawalMade> Handle2 = (e) =>
            {
                eventBus.Publish<DepositMade>(new DepositMade(Guid.NewGuid(), 20));
            };

            eventBus.Subscribe<DepositMade>(Handle);
            eventBus.Subscribe<WithdrawalMade>(Handle2);

            eventBus.Publish<DepositMade>(new DepositMade(Guid.NewGuid(), 40));
        }

        [TestMethod]
        public void ShouldAllowChainedUnidirectionalConcurrentPublishes()
        {
            EventBus eventBus = new EventBus();

            int timesTriggered = 0;

            Action<AccountCreated> Handle = (e) =>
            {
                eventBus.Publish<DepositMade>(new DepositMade(e.AggregateId, 20));
            };

            Action<DepositMade> Handle2 = (e) =>
            {
                eventBus.Publish<WithdrawalMade>(new WithdrawalMade(e.AggregateId, 20));
            };

            Action<WithdrawalMade> Handle3 = (e) =>
            {
                timesTriggered++;
            };

            eventBus.Subscribe<AccountCreated>(Handle);
            eventBus.Subscribe<DepositMade>(Handle2);
            eventBus.Subscribe<WithdrawalMade>(Handle3);

            eventBus.Publish<AccountCreated>(new AccountCreated(Guid.NewGuid(), "Test"));
            Assert.AreEqual(1, timesTriggered);
            eventBus.Publish<AccountCreated>(new AccountCreated(Guid.NewGuid(), "Test2"));
            Assert.AreEqual(1, timesTriggered);
        }

        //// Very difficult to enforce unless each handler wrapped
        //// in an object with identity.
        //[TestMethod]
        //[ExpectedException(typeof(Exception))]
        //public void ShouldThrowExceptionWhenAttemptingToSubscribeMoreThanOnce()
        //{
        //    Action<AccountCreated> Handle = (e) => { };

        //    IEventSubscription eventBus = new EventBus();
        //    eventBus.Subscribe(Handle);
        //    eventBus.Subscribe(Handle);
        //}
    }
}
