using System;
using System.Collections.Generic;

namespace Arian
{
    public class bl_Event
    {
        private Dictionary<System.Type, IEventContainer> eventContainer = new Dictionary<System.Type, IEventContainer>();
        public static readonly bl_Event Global = new bl_Event();

        public void AddListener<T>(Action<T> callback)
        {
            IEventContainer container;
            if (!this.eventContainer.TryGetValue(typeof(T), out container))
            {
                container = new EventContainer<T>();
                this.eventContainer.Add(typeof(T), container);
            }
            EventContainer<T> container2 = container as EventContainer<T>;
            if (container2 != null)
            {
                container2.AddCallbackMethod(callback);
            }
        }

        public void Clear()
        {
            this.eventContainer.Clear();
        }

        public void DispatchEvent(object message)
        {
            IEventContainer container;
            if (this.eventContainer.TryGetValue(message.GetType(), out container))
            {
                container.CastEvent(message);
            }
        }

        public void RemoveListener<T>(Action<T> callback)
        {
            IEventContainer container;
            if (this.eventContainer.TryGetValue(typeof(T), out container))
            {
                EventContainer<T> container2 = container as EventContainer<T>;
                if (container2 != null)
                {
                    container2.RemoveCallbackMethod(callback);
                }
            }
        }

        private class EventContainer<T> : bl_Event.IEventContainer
        {
            private Dictionary<string, Action<T>> _dictionary;

            public event Action<T> Sender;

            public EventContainer()
            {
                this._dictionary = new Dictionary<string, Action<T>>();
            }

            public void AddCallbackMethod(Action<T> callback)
            {
                string callbackMethodId = this.GetCallbackMethodId(callback);
                if (!this._dictionary.ContainsKey(callbackMethodId))
                {
                    this._dictionary.Add(callbackMethodId, callback);
                    this.Sender = (Action<T>)Delegate.Combine(this.Sender, callback);
                }
            }

            public void CastEvent(object m)
            {
                if (this.Sender != null)
                {
                    this.Sender((T)m);
                }
            }

            private string GetCallbackMethodId(Action<T> callback)
            {
                string str = callback.Method.DeclaringType.FullName + callback.Method.Name;
                if (callback.Target != null)
                {
                    str = str + callback.Target.GetHashCode().ToString();
                }
                return str;
            }

            public void RemoveCallbackMethod(Action<T> callback)
            {
                if (this._dictionary.Remove(this.GetCallbackMethodId(callback)))
                {
                    this.Sender = (Action<T>)Delegate.Remove(this.Sender, callback);
                }
            }
        }

        private interface IEventContainer
        {
            void CastEvent(object m);
        }
    }

}