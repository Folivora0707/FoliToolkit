using System;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

namespace Foli
{
    public sealed class Countdown
    {
        private const double Epsilon = 1E-06;
        
        private static readonly List<Countdown> Timers = new();
        private static Driver _driver;

        private Func<double> _now;
        private Func<double> _end;

        private Action<double> _onTick;
        private Action<double> _onComplete;
        
        private double _interval;

        private bool _loop;
        private bool _running;


        /// <summary>
        /// 计时启动
        /// </summary>
        /// <param name="end"> 获取结束时间戳方法 </param>
        /// <param name="onTick"> 心跳回调 </param>
        /// <param name="onComplete"> 完成回调 </param>
        /// <param name="now"> 获取当前时间戳的方法 </param>
        /// <param name="loop"> 是否循环 </param>
        /// <param name="interval"> 心跳间隔 </param>
        /// <returns></returns>
        public static Countdown Start(
            Func<double> end,
            Action<string, double> onTick = null,
            Action onComplete = null,
            Func<double> now = null,
            bool loop = false,
            double interval = 1d)
        {
            if (end == null) return null;
            if (onTick == null && onComplete == null) return null;
            
            var countdown = new Countdown();
            return countdown;
        }

        public void Stop()
        {
            
        }

        private void Tick(bool force = false)
        {
            
        }

        private static void EnsureDriver()
        {
            if (_driver) return;
            var go = new GameObject("[Countdown]") { hideFlags = HideFlags.HideInHierarchy };
            UnityEngine.Object.DontDestroyOnLoad(go);
            _driver = go.AddComponent<Driver>();
        }
        
        private static double DefaultNow()
        {
            return Time.realtimeSinceStartupAsDouble;
        }
        
        public static string Format(double seconds)
        {
            var total = Mathf.CeilToInt((float)Math.Max(0d, seconds));
            return $"{total / 3600: D2}:{total % 3600 / 60: D2}:{total % 60: D2}";
        }

        private sealed class Driver : MonoBehaviour
        {
            private void Update()
            {
                for (var i = Timers.Count - 1; i >= 0; i--)
                {
                    var timer = Timers[i];
                    if (!timer._running)
                    {
                        Timers.RemoveAt(i);
                        continue;
                    }

                    timer.Tick();
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
