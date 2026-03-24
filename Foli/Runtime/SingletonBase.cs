using UnityEngine;

namespace Foli
{
    public static class AppState
    {
        public static bool IsQuitting;
    }
    
    public abstract class SingletonBase<T> : MonoBehaviour where T : SingletonBase<T>
    {
        private static T _instance;
        private bool _initialized;
        
        public static T Instance
        {
            get
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    return null;
#endif
                if (AppState.IsQuitting) 
                    return null;
                
                if (_instance == null)
                    _instance = CreateInstance();
                
                return _instance;
            }
        }
        
        private static T CreateInstance()
        {
            var go = new GameObject(typeof(T).Name);
            var instance = go.AddComponent<T>();
            DontDestroyOnLoad(go);
            return instance;
        }
        private void InitializeOnce()
        {
            if (_initialized) return;
            _initialized = true;
            OnSingletonInit();
        }
        protected virtual void OnSingletonInit() { }
        
        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
                InitializeOnce();
            }
            else if (_instance != this)
                Destroy(gameObject);
        }
        protected virtual void OnDestroy()
        {
            if (_instance == this && !AppState.IsQuitting)
                _instance = null;
        }
        protected virtual void OnApplicationQuit()
        {
            AppState.IsQuitting = true;
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            _instance = null;
            AppState.IsQuitting = false;
        }
    }
}

