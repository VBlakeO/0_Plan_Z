using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace PlanZ.Events
{
    public static class EventBus
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void ClearAllGenericBusses()
        {
            var eventTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsClass && !type.IsAbstract && typeof(IEvent).IsAssignableFrom(type));

            var eventBusType = typeof(GenericEventBus<>);

            foreach (var eventType in eventTypes)
            {
                var type = eventBusType.MakeGenericType(eventType);
                var method = type.GetMethod("Clear");
                method?.Invoke(null, null);
            }
        }

        public static void Subscribe<T>(Action<T> handler, int priority = 0) where T : IEvent =>
            GenericEventBus<T>.Subscribe(handler, priority);

        public static void Subscribe<T>(Action handler, int priority = 0) where T : IEvent =>
            GenericEventBus<T>.Subscribe(handler, priority);

        public static void Unsubscribe<T>(Action<T> handler) where T : IEvent =>
            GenericEventBus<T>.Unsubscribe(handler);

        public static void Unsubscribe<T>(Action handler) where T : IEvent =>
            GenericEventBus<T>.Unsubscribe(handler);

        public static void Publish<T>(T eventData) where T : IEvent =>
            GenericEventBus<T>.Publish(eventData);

        private static class GenericEventBus<T> where T : IEvent
        {
            private static readonly List<IEventBinding<T>> EVENT_BINDINGS = new();
            private static readonly Dictionary<Action<T>, IEventBinding<T>> TYPED_BINDINGS = new();
            private static readonly Dictionary<Action, IEventBinding<T>> NO_ARG_BINDINGS = new();

            [UsedImplicitly]
            public static void Clear()
            {
                EVENT_BINDINGS.Clear();
                TYPED_BINDINGS.Clear();
                NO_ARG_BINDINGS.Clear();
            }

            private static void Subscribe(IEventBinding<T> eventBinding)
            {
                EVENT_BINDINGS.Add(eventBinding);
                SortEventBindings();
            }

            public static void Subscribe(Action<T> handler, int priority)
            {
                if (TYPED_BINDINGS.ContainsKey(handler))
                {
                    return;
                }

                var eventBinding = new EventBinding<T>(handler, priority);
                TYPED_BINDINGS[handler] = eventBinding;
                Subscribe(eventBinding);
            }

            public static void Subscribe(Action handler, int priority)
            {
                if (NO_ARG_BINDINGS.ContainsKey(handler))
                {
                    return;
                }

                var eventBinding = new EventBinding<T>(handler, priority);
                NO_ARG_BINDINGS[handler] = eventBinding;
                Subscribe(eventBinding);
            }

            private static void SortEventBindings()
            {
                var orderedEnumerable = EVENT_BINDINGS
                    .OrderByDescending(eventBinding => eventBinding.Priority)
                    .ToArray();

                EVENT_BINDINGS.Clear();

                foreach (var eventBinding in orderedEnumerable)
                {
                    EVENT_BINDINGS.Add(eventBinding);
                }
            }

            private static void Unsubscribe(IEventBinding<T> eventBinding) => EVENT_BINDINGS.Remove(eventBinding);

            public static void Unsubscribe(Action<T> handler)
            {
                if (!TYPED_BINDINGS.Remove(handler, out var eventBinding))
                {
                    return;
                }

                Unsubscribe(eventBinding);
            }

            public static void Unsubscribe(Action handler)
            {
                if (!NO_ARG_BINDINGS.Remove(handler, out var eventBinding))
                {
                    return;
                }

                Unsubscribe(eventBinding);
            }

            public static void Publish(T eventData)
            {
                // A For is used here rather than a Foreach because the event bindings can be modified
                // during the invocation of the handlers. This prevents a potential InvalidOperationException
                // that can occur when modifying a collection while iterating over it.
                // ReSharper disable once ForCanBeConvertedToForeach
                for (var i = 0; i < EVENT_BINDINGS.Count; i++)
                {
                    var eventBinding = EVENT_BINDINGS[i];
                    eventBinding.Handler?.Invoke(eventData);
                    eventBinding.HandlerNoArgs?.Invoke();
                }
            }
        }
    }
}
