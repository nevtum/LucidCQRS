using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LucidCQRS.Messaging.Commanding;
using LucidCQRS.Messaging.Exceptions;

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
        [ExpectedException(typeof(HandlerMissingException))]
        public void ShouldThrowExceptionIfMissingHandlerRegistered()
        {
            Action<PerformSpecialOperation> Handle1 = delegate { };
            Action<PerformAnotherOperation> Handle2 = delegate { };

            CommandBus commandBus = new CommandBus();
            commandBus.RegisterHandler(Handle1);

            commandBus.Send(new PerformSpecialOperation()); // Should pass
            commandBus.Send(new PerformAnotherOperation()); // Should throw exception
        }

        [TestMethod]
        [ExpectedException(typeof(DuplicateHandlerException))]
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

    // Helper class only
    public class PerformAnotherOperation : Command
    {
        public PerformAnotherOperation()
            : base(Guid.NewGuid())
        {
        }
    }
}
