using System;

namespace PlanZ.Events
{
    public interface IEventBinding<in T>
    {
        public int Priority { get; }
        public Action<T> Handler { get; }
        public Action HandlerNoArgs { get; }
    }
}
