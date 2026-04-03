using System;
using System.Collections.Generic;

namespace Foli
{
    public class EventDomain
    {
        private readonly Dictionary<Type, object> _handlers = new();

        /// <summary>
        /// 订阅事件监听
        /// </summary>
        /// <param name="callback"> 事件回调 </param>
        /// <param name="priority"> 事件优先级 </param>
        /// <typeparam name="T"> 事件类型 </typeparam>
        public void Subscribe<T>(Action<T> callback, int priority = 0)
        {
            var type = typeof(T);
            if (!_handlers.TryGetValue(type, out var handler))
            {
                handler = new EventHandler<T>();
                _handlers[type] = handler;
            }

            ((EventHandler<T>)handler).Add(callback, priority);
        }

        /// <summary>
        /// 取消订阅事件监听
        /// </summary>
        /// <param name="callback"> 事件回调 </param>
        /// <typeparam name="T"> 事件类型 </typeparam>
        public void Unsubscribe<T>(Action<T> callback)
        {
            if (_handlers.TryGetValue(typeof(T), out var handler))
                ((EventHandler<T>)handler).Remove(callback);
        }
        
        /// <summary>
        /// 发布事件
        /// </summary>
        /// <param name="e"> 可带参数的事件实例 </param>
        /// <typeparam name="T"> 事件类型 </typeparam>
        public void Publish<T>(T e)
        {
            if (_handlers.TryGetValue(typeof(T), out var handler))
                ((EventHandler<T>)handler).Invoke(e);
        }
        
        /// <summary>
        /// 清空事件域订阅
        /// </summary>
        public void Clear()
        {
            _handlers.Clear();
        }
    }
}