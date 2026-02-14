using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foli
{
    /// <summary>
    /// 全局协程执行器
    /// </summary>
    public class CoroutineManager : SingletonBase<CoroutineManager>
    {
        private static readonly Dictionary<object, Coroutine> KeyMap = new();
        private static readonly Dictionary<string, List<Coroutine>> TagMap = new();

        #region 启动

        /// <summary>
        /// 原生协程启动入口
        /// </summary>
        public static Coroutine Run(IEnumerator routine)
        {
            return routine == null ? null : Instance.StartCoroutine(routine);
        }
        /// <summary>
        /// 带异常保护，有性能开销
        /// </summary>
        public static Coroutine RunSafe(IEnumerator routine)
        {
            return routine == null ? null : Run(WrapWithTryCatch(routine));
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

        public static Coroutine RunUnique(object key, IEnumerator routine, bool safe = false)
        {
            if (key == null) return safe ? RunSafe(routine) : Run(routine);

            if (KeyMap.TryGetValue(key, out var old))
            {
                if (old != null) Instance.StopCoroutine(old);
            }

            routine = WrapWithCallback(routine, () => { StopUnique(key); });
            var c = safe ? RunSafe(routine) : Run(routine);
            KeyMap[key] = c;
            return c;
        }

        public static void BindUnique(object key, Coroutine coroutine)
        {
            if (key == null ||  coroutine == null) return;
            if (KeyMap.TryGetValue(key, out var old))
            {
                if (old != null) Instance.StopCoroutine(old);
            }
            KeyMap[key] = coroutine;
        }

        public static void StopUnique(object key)
        {
            if (key == null) return;
            if (KeyMap.TryGetValue(key, out var coroutine))
            {
                if (coroutine != null) Instance.StopCoroutine(coroutine);
                KeyMap.Remove(key);
            }
        }

        #endregion

        #region 统一 Tag 管理

        public static Coroutine RunTag(string tag, IEnumerator routine, bool safe = false)
        {
            if (string.IsNullOrEmpty(tag)) return safe ? RunSafe(routine) : Run(routine);

            routine = WrapWithCallback(routine, () =>
            {
  
            });
            var c = safe ? RunSafe(routine) : Run(routine);
            if (!TagMap.TryGetValue(tag, out var list))
            {
                list = new List<Coroutine>();
                TagMap[tag] = list;
            }
            list.Add(c);
            return c;
        }

        public static void BindTag(string tag, Coroutine coroutine)
        {
            if (string.IsNullOrEmpty(tag) || coroutine == null) return;

            if (!TagMap.TryGetValue(tag, out var list))
            {
                list = new List<Coroutine>();
                TagMap[tag] = list;
            }
            list.Add(coroutine);
        }
        
        public static void StopTag(string tag)
        {
            if (string.IsNullOrEmpty(tag)) return;
            if (!TagMap.TryGetValue(tag, out var list)) return;
            foreach (var c in list)
            {
                if (c != null) Instance.StopCoroutine(c);
            }
            list.Clear();
        }

        #endregion

        #region 返回协程的 Delay

        public static Coroutine Delay(float seconds, Action callback, bool realtime = false, bool safe = false)
        {
            var routine = DelayRoutine(seconds, callback, realtime);
            return safe ? RunSafe(routine) : Run(routine);
        }

        private static IEnumerator DelayRoutine(float seconds, Action callback, bool realtime)
        {
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
        
        #endregion
        
    }
}

