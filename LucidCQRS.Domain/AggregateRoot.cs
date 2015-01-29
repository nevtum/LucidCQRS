using System;
using System.Collections.Generic;
using System.Reflection;
using LucidCQRS.Common;

namespace LucidCQRS.Domain
{
    public abstract class AggregateRoot
    {
        private readonly List<Event> _newChanges = new List<Event>();

        public Guid Id { get; private set; }

        public AggregateRoot(Guid id)
        {
            Id = id;
        }

        public IEnumerable<Event> GetUncommittedChanges()
        {
            return _newChanges;
        }

        public void MarkChangesAsCommitted()
        {
            _newChanges.Clear();
        }

        public void LoadFromHistory(IEnumerable<Event> history)
        {
            foreach (Event e in history)
                ApplyChange(e);
        }

        protected void ApplyNewChange(Event e)
        {
            ApplyChange(e);
            _newChanges.Add(e);
        }

        private void ApplyChange(Event e)
        {
            // Using reflection to match up method with concrete parameter.
            // All derived classes must follow convention by containing
            // the "Apply" method signature with matching concrete event param.
            Type type = GetType();
            MethodInfo methodInfo = type.GetMethod("Apply", new[] { e.GetType() });

            // make it fail if derived class does not contain method name
            methodInfo.Invoke(this, new[] { e });
        }
    }
}
