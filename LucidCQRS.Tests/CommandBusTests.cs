using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LucidCQRS.Messaging.Commanding;

namespace LucidCQRS.Tests
{
    [TestClass]
    public class CommandBusTests
    {
        [TestMethod]
        public void ShouldTriggerSubscribedHandlerWhenCommandIsSent()
        {
            bool triggered = false;

            Action<PerformSpecialOperation> Handle = (c) =>
            {
                triggered = true;
            };

            CommandBus commandBus = new CommandBus();
            commandBus.RegisterHandler<PerformSpecialOperation>(Handle);
            commandBus.Send(new PerformSpecialOperation());

            Assert.AreEqual(true, triggered);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ShouldThrowExceptionWhenSubscribingMultipleHandlersToSameCommand()
        {
            CommandBus commandBus = new CommandBus();
            commandBus.RegisterHandler<PerformSpecialOperation>(Handle1);
            commandBus.RegisterHandler<PerformSpecialOperation>(Handle2);
        }

        // Helper method only
        private void Handle2(PerformSpecialOperation obj)
        {
        }

        // Helper method only
        private void Handle1(PerformSpecialOperation obj)
        {
        }
    }

    // Helper class only
    public class PerformSpecialOperation : Command
    {
        public PerformSpecialOperation()
            : base(Guid.NewGuid())
        {
        }
    }
}
