using UnityEngine;

namespace Foli
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T _instance;
        private static bool _isQuitting;

        /// <summary>
        /// 显式初始化，确保单例初始顺序
        /// </summary>
        public static void Init() => _ = Instance;
        
        /// <summary>
        /// 获取单例引用
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_isQuitting) return null;
                if (_instance == null)
                    _instance = CreateInstance(); // 懒初始化
                return _instance;
            }
        }

        // 动态创建对象
        private static T CreateInstance()
        {
            var go = new GameObject(typeof(T).Name);
            var instance = go.AddComponent<T>();
            DontDestroyOnLoad(go);
            return instance;
        }

        // 确保单例唯一
        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
                Destroy(gameObject);
        }

        // 清理引用
        protected virtual void OnDestroy()
        {
            if (_instance == this) _instance = null;
        }
        
        // 退出标记
        protected virtual void OnApplicationQuit()
        {
            _isQuitting = true;
        }
    }
}

