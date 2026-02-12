using System.Collections;
using UnityEngine;

namespace Foli.Unity
{
    public class CoroutineRunner : MonoBehaviour
    {
        private static CoroutineRunner _instance;

        private static CoroutineRunner Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject("CoroutineRunner");
                    _instance = go.AddComponent<CoroutineRunner>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        public static Coroutine Run(IEnumerator coroutine) => Instance.StartCoroutine(coroutine);
        
    }
}

