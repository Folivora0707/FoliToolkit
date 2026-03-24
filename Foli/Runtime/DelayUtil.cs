using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Foli
{
    public static class DelayUtil
    {
        
        #region 对外接口

        /// <summary>
        /// 延时 N 秒执行（可取消）
        /// </summary>
        /// <returns> 控制取消的句柄 </returns>
        public static DelayHandle Run(Action action, float seconds, bool ignoreTimeScale = false)
        {
            var cts = new CancellationTokenSource();
            int ms = Mathf.Max(1, Mathf.RoundToInt(seconds * 1000f));
            
            Delay(action, ms, cts, ignoreTimeScale).Forget();
            return new DelayHandle(cts);
        }
        
        /// <summary>
        /// 延时 N 秒执行
        /// </summary>
        public static void RunDelay(Action action, float seconds, bool ignoreTimeScale = false)
        {
            int milliseconds = Mathf.RoundToInt(seconds * 1000);
            Delay(action, milliseconds, ignoreTimeScale).Forget();
        }

        /// <summary>
        /// 延时 N 帧执行
        /// </summary>
        public static void RunDelayFrame(Action action, int frameCount = 1)
        {
            DelayFrame(action, frameCount).Forget();
        }

        /// <summary>
        /// 延时至帧尾执行
        /// </summary>
        public static void RunDelay2FrameEnd(Action action)
        {
            Delay2FrameEnd(action).Forget();
        }

        #endregion

        #region 内部实现

        public sealed class DelayHandle : IDisposable
        {
            private CancellationTokenSource _cts;

            internal DelayHandle(CancellationTokenSource cts) => _cts = cts;

            public void Cancel()
            {
                _cts?.Cancel();
                _cts = null;
            }

            public void Dispose() => Cancel();
        }
        
        /// <summary>
        /// 延时 N 毫秒执行某方法（可取消）
        /// </summary>
        private static async UniTask Delay(Action action, int milliseconds, CancellationTokenSource cts, bool ignoreTimeScale)
        {
            var delayType = ignoreTimeScale ? DelayType.UnscaledDeltaTime : DelayType.DeltaTime;
            try
            {
                if (await UniTask.Delay(milliseconds, delayType, cancellationToken: cts.Token)
                        .SuppressCancellationThrow())
                    return;
                
                action?.Invoke();
            }
            finally
            {
                cts.Dispose();
            }
        }
        
        /// <summary>
        /// 延时 N 毫秒执行某方法
        /// </summary>
        /// <param name="action"> 方法 </param>
        /// <param name="milliseconds"> 毫秒数 </param>
        /// <param name="ignoreTimeScale">忽略 TimeScale 影响</param>
        private static async UniTask Delay(Action action, int milliseconds, bool ignoreTimeScale = false)
        {
            await UniTask.Delay(milliseconds, ignoreTimeScale: ignoreTimeScale);
            action?.Invoke();
        }

        /// <summary>
        /// 延时 N 帧执行某方法
        /// </summary>
        /// <param name="action"> 方法 </param>
        /// <param name="frameCount"> 帧数，默认延时一帧 </param>
        private static async UniTask DelayFrame(Action action, int frameCount = 1)
        {
            for (var i = 0; i < frameCount; ++i)
                await UniTask.Yield();
            action?.Invoke();
        }

        /// <summary>
        /// 延时到帧尾执行方法
        /// </summary>
        /// <param name="action"> 方法 </param>
        private static async UniTask Delay2FrameEnd(Action action)
        {
            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
            action?.Invoke();
        }

        #endregion

    }
}
