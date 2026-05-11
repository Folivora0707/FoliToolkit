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
            var handle = new DelayHandle();
            int ms = Mathf.Max(1, Mathf.RoundToInt(seconds * 1000f));
            
            Delay(action, ms, handle, ignoreTimeScale).Forget();
            return handle;
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

        public sealed class DelayHandle
        {
            private CancellationTokenSource _cts = new();
            internal CancellationToken Token => _cts?.Token ?? CancellationToken.None;

            private void Release(bool cancel)
            {
                var cts = _cts;
                _cts = null;

                if (cts == null) return;
                try
                {
                    if (cancel) cts.Cancel();
                }
                finally
                {
                    cts.Dispose();
                }
            }

            /// <summary> 取消未完成的延时任务 </summary>
            public void Cancel() => Release(true);

            /// <summary> 延时任务已自然结束，释放资源 </summary>
            internal void Complete() => Release(false);
        }
        
        /// <summary>
        /// 延时 N 毫秒执行某方法（可取消）
        /// </summary>
        private static async UniTask Delay(Action action, int milliseconds, DelayHandle handle, bool ignoreTimeScale)
        {
            var delayType = ignoreTimeScale ? DelayType.UnscaledDeltaTime : DelayType.DeltaTime;
            try
            {
                if (await UniTask.Delay(milliseconds, delayType, cancellationToken: handle.Token)
                        .SuppressCancellationThrow())
                    return;
                
                action?.Invoke();
            }
            finally
            {
                handle.Complete();
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
