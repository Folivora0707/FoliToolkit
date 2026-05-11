using System;
using System.Collections.Generic;
using UnityEngine;

namespace Foli
{
    public sealed class Countdown
    { 
        private static readonly List<Countdown> Timers = new();
        private static Driver _driver;

        private Func<double> _end;
        private Func<double> _now;
        private Action<string, double> _onTick;
        private Action _onComplete;
        
        private double _interval;
        private double _nextTick;
        private bool _running;

        /// <summary>
        /// 计时启动
        /// </summary>
        /// <param name="end"> 获取结束时间戳方法 </param>
        /// <param name="onTick"> 心跳回调 </param>
        /// <param name="onComplete"> 完成回调 </param>
        /// <param name="now"> 获取当前时间戳的方法 </param>
        /// <param name="interval"> 心跳间隔 </param>
        /// <returns></returns>
        public static Countdown Start(
            Func<double> end,
            Action<string, double> onTick = null,
            Action onComplete = null,
            Func<double> now = null,
            double interval = 1d)
        {
            if (end == null) return null;
            if (onTick == null && onComplete == null) return null;
            
            EnsureDriver();

            var countdown = new Countdown
            {
                _end = end,
                _now = now ?? (() => Time.realtimeSinceStartupAsDouble),
                _onTick = onTick,
                _onComplete = onComplete,
                _interval = Math.Max(0d, interval),
                _running = true
            };
            
            Timers.Add(countdown);
            countdown.Tick();
            
            return countdown;
        }

        public void Stop()
        {
            if (!_running) return;
            
            _running = false;
            _end = null;
            _now = null;
            _onTick = null;
            _onComplete = null;
        }

        private void Tick()
        {
            if(!_running) return;
            
            var now = _now();
            var remain = Math.Max(0d, _end() - now);
            if (now >= _nextTick)
            {
                _nextTick = now + _interval;
                _onTick?.Invoke(Format(remain), remain);
            }

            if (remain > 0d) return;

            var cache = _onComplete;
            Stop();
            cache?.Invoke();
        }

        private static void EnsureDriver()
        {
            if (_driver) return;
            
            var go = new GameObject("[Countdown]") { hideFlags = HideFlags.HideInHierarchy };
            UnityEngine.Object.DontDestroyOnLoad(go);
            _driver = go.AddComponent<Driver>();
        }
        
        private static string Format(double seconds)
        {
            var total = Mathf.CeilToInt((float)Math.Max(0d, seconds));
            return $"{total / 3600:D2}:{total % 3600 / 60:D2}:{total % 60:D2}";
        }

        private sealed class Driver : MonoBehaviour
        {
            private void Update()
            {
                for (var i = Timers.Count - 1; i >= 0; i--)
                {
                    var timer = Timers[i];
                    if (timer._running)
                    {
                        timer.Tick();
                        continue;
                    }

                    var tail = Timers.Count - 1;
                    Timers[i] = Timers[tail];
                    Timers.RemoveAt(tail);
                }
            }

            private void OnDestroy()
            {
                if (_driver == this) _driver = null;
                Timers.Clear();
            }
        }
    }
}
