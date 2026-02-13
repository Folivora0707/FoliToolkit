using System.Collections;
using UnityEngine;

namespace Foli
{
    /// <summary>
    /// 全局协程执行器
    /// </summary>
    public class CoroutineRunner : MonoSingleton<CoroutineRunner>
    {
        
        public static Coroutine Run(IEnumerator coroutine) => Instance.StartCoroutine(coroutine);
        
    }
}

