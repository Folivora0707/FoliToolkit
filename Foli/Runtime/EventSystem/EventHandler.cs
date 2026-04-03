using System;
using System.Collections.Generic;

namespace Foli
{
    public class EventHandler<T>
    {
        private class Listener
        {
            public Action<T> Callback;
            public int Priority;
            public int Order;
        }
        
        private readonly List<Listener> _listeners = new();
        private readonly List<Listener> _invokeCache = new();
        
        private bool _dirty;
        private int _order;
        
        public bool IsEmpty => _listeners.Count == 0;

        public void Add(Action<T> callback, int priority)
        {
            _listeners.Add(new Listener { Callback = callback, Priority = priority, Order = ++_order });
            _dirty = true;
        }

        public void Remove(Action<T> callback)
        {
            for (var i = _listeners.Count - 1; i >= 0; i--)
            {
                if (_listeners[i].Callback == callback)
                {
                    _listeners.RemoveAt(i);
                    _dirty = true;
                }
            }
        }

        public void Invoke(T e)
        {
            if (_listeners.Count == 0) return;
            
            if (_dirty)
            {
                _listeners.Sort((x, y) =>
                {
                    var priorityCompare = y.Priority.CompareTo(x.Priority);
                    return priorityCompare != 0 ? priorityCompare : x.Order.CompareTo(y.Order);
                });
                _dirty = false;
            }

            _invokeCache.Clear();
            _invokeCache.AddRange(_listeners);
            foreach (var listener in _invokeCache)
            {
                try
                {
                    listener.Callback?.Invoke(e);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }
        }
    }
}