using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public static class DelayUtil
{
    #region 对外接口，隐藏 Forget 语义
        
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

    #endregion 对外接口
        
    #region 内部实现

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
        
    #endregion 内部实现
} // [End] DelayUtil
