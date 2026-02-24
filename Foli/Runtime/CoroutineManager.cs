using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foli
{
    /// <summary>
    /// 全局协程管理器
    /// </summary>
    public class CoroutineManager : SingletonBase<CoroutineManager>
    {
        private class CoroutineHandle
        {
            public Coroutine Coroutine;
            public bool IsRunning;
        }
        private static readonly Dictionary<object, CoroutineHandle> KeyMap = new();
        private static readonly Dictionary<string, List<CoroutineHandle>> TagMap = new();

        #region 启动

        /// <summary>
        /// 支持装饰器包装后再调用
        /// </summary>
        public static Coroutine Run(IEnumerator routine)
        {
            return routine == null ? null : Instance.StartCoroutine(routine);
        }

        private static CoroutineHandle RunTracked(IEnumerator routine)
        {
            var handle = new CoroutineHandle();
            handle.Coroutine = Instance.StartCoroutine(WrapIsRunning(routine, handle));
            return handle;
        }

        #endregion

        #region 停止

        public static void Stop(Coroutine coroutine)
        {
            if (coroutine == null) return;
            Instance.StopCoroutine(coroutine);
        }

        public static void StopAll()
        {
            Instance.StopAllCoroutines();
            KeyMap.Clear();
            TagMap.Clear();
        }

        #endregion

        #region 唯一 Key 管理

        public static Coroutine RunUnique(object key, IEnumerator routine)
        {
            if (key == null) return Run(routine); // key 为 null 时，等效于原生协程，不管理

            if (KeyMap.TryGetValue(key, out var old) && old != null)
                Instance.StopCoroutine(old.Coroutine);

            var handle = new CoroutineHandle();
            var wrapper = WrapAutoCleanKey(routine, handle, key);
            handle = RunTracked(wrapper);
            KeyMap[key] = handle;
            return handle.Coroutine;
        }

        public static bool IsRunningUnique(object key)
        {
            return key != null && KeyMap.TryGetValue(key, out var handle) && handle.IsRunning;
        }
        
        public static void StopUnique(object key)
        {
            if (key == null) return;
            if (KeyMap.TryGetValue(key, out var handle))
            {
                if (handle?.Coroutine != null) Instance.StopCoroutine(handle.Coroutine);
                KeyMap.Remove(key);
            }
        }

        #endregion

        #region 统一 Tag 管理

        public static Coroutine RunTag(string tag, IEnumerator routine)
        {
            if (string.IsNullOrEmpty(tag)) return Run(routine); // tag 为 null 时，等效于原生协程，不管理
            
            var handle = new CoroutineHandle();
            var wrapper = WrapAutoCleanTag(routine, handle, tag);
            handle = RunTracked(wrapper);
            
            if (!TagMap.TryGetValue(tag, out var list))
            {
                list = new List<CoroutineHandle>();
                TagMap[tag] = list;
            }
            if (!list.Contains(handle))
                list.Add(handle);
            return handle.Coroutine;
        }

        public static bool IsRunningTag(string tag)
        {
            if (string.IsNullOrEmpty(tag)) return false;
            if (!TagMap.TryGetValue(tag, out var list)) return false;
            
            foreach (var handle in list)
            {
                if (handle.IsRunning)
                    return true;
            }
            
            return false;
        }
        
        public static void StopTag(string tag)
        {
            if (string.IsNullOrEmpty(tag)) return;
            if (!TagMap.TryGetValue(tag, out var list)) return;
            
            foreach (var handle in list)
            {
                if (handle is { Coroutine: not null })
                    Instance.StopCoroutine(handle.Coroutine);
            }
            list.Clear();
            TagMap.Remove(tag);
        }

        #endregion

        #region 支持 yield return Delay

        public static IEnumerator Delay(float seconds, Action callback, bool realtime = false)
        {
            seconds = Mathf.Max(seconds, 0.0f);
            if (realtime)
                yield return new WaitForSecondsRealtime(seconds);
            else
                yield return new WaitForSeconds(seconds);
            callback?.Invoke();
        }

        #endregion

        #region Routine 装饰器
        
        private static IEnumerator WrapWithCallback(IEnumerator routine, Action callback)
        {
            while (routine.MoveNext())
            {
                yield return routine.Current;
            }
            callback?.Invoke();
        }
        
        private static IEnumerator WrapWithTryCatch(IEnumerator routine)
        {
            while (true)
            {
                object current;
                try
                {
                    if (!routine.MoveNext())
                        yield break;
                    current = routine.Current;
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    yield break;
                }
                yield return current;
            }
        }

        // Key 自动清理
        private static IEnumerator WrapAutoCleanKey(IEnumerator routine, CoroutineHandle handle, object key)
        {
            yield return routine;
            if (KeyMap.TryGetValue(key, out var h) && h == handle)
                KeyMap.Remove(key);
        }

        // Tag 自动清理
        private static IEnumerator WrapAutoCleanTag(IEnumerator routine, CoroutineHandle handle, string tag)
        {
            yield return routine;
            if (TagMap.TryGetValue(tag, out var list))
            {
                list.Remove(handle);
                if (list.Count == 0) TagMap.Remove(tag);
            }
        }

        // 运行中标记
        private static IEnumerator WrapIsRunning(IEnumerator routine, CoroutineHandle handle)
        {
            handle.IsRunning = true;
            yield return routine;
            handle.IsRunning = false;
        }

        #endregion
        
    }
}

