﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LucidCQRS.Messaging.Eventing;
using LucidCQRS.Tests.Accounting;

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
        public void ShouldTriggerMultipleSubscribersWhenEventIsPublished()
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
        public void ShouldNotTriggerUnsubscribedHandlerWhenEventIsPublished()
        {
            bool triggered = false;

            Action<AccountCreated> Handle = (e) =>
            {
                triggered = true;
            };

            EventBus eventBus = new EventBus();
            eventBus.Subscribe(Handle);
            eventBus.Unsubscribe(Handle);
            eventBus.Publish(new AccountCreated(Guid.NewGuid(), "Test"));

            Assert.AreEqual(false, triggered);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ShouldThrowExceptionWhenAttemptingToSubscribeMoreThanOnce()
        {
            Action<AccountCreated> Handle = (e) => { };

            IEventSubscription eventBus = new EventBus();
            eventBus.Subscribe(Handle);
            eventBus.Subscribe(Handle);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ShouldThrowExceptionOnUnsubscribeWhenNoSubscriptionsExist()
        {
            Action<AccountCreated> Handle = (e) => { };

            IEventSubscription eventBus = new EventBus();
            eventBus.Unsubscribe(Handle);
        }
    }
}
