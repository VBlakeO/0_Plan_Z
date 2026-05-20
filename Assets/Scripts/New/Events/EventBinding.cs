using System;

namespace PlanZ.Events
{
    public class EventBinding<T> : IEventBinding<T> where T : IEvent
    {
        public int Priority { get; private set; }
        public Action<T> Handler { get; private set; }
        public Action HandlerNoArgs { get; private set; }

        public EventBinding(Action<T> handler, int priority = 0)
        {
            Handler = handler;
            Priority = priority;
        }

        public EventBinding(Action handlerNoArgs, int priority = 0)
        {
            HandlerNoArgs = handlerNoArgs;
            Priority = priority;
        }

        public EventBinding(Action<T> handler, Action handlerNoArgs, int priority = 0)
        {
            Handler = handler;
            HandlerNoArgs = handlerNoArgs;
            Priority = priority;
        }

        public void Add(Action<T> handler) => Handler += handler;

        public void Add(Action handler) => HandlerNoArgs += handler;

        public void Remove(Action<T> handler) => Handler -= handler;

        public void Remove(Action handler) => HandlerNoArgs -= handler;
    }
}
