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
        }
        
        private readonly List<Listener> _listeners = new();
        private readonly List<Listener> _invokeCache = new();
        
        private bool _dirty;

        public void Add(Action<T> callback, int priority)
        {
            _listeners.Add(new Listener { Callback = callback, Priority = priority });
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
                _listeners.Sort((x, y) => y.Priority.CompareTo(x.Priority));
                _dirty = false;
            }

            _invokeCache.Clear();
            _invokeCache.AddRange(_listeners);
            foreach (var listener in _invokeCache)
                listener.Callback?.Invoke(e);
        }
    }
}