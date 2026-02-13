// using UnityEngine;
//
// namespace Foli
// {
//     public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
//     {
//         
//         private static T _instance;
//         private static bool _isQuitting;
//
//         /// <summary>
//         /// 显式初始化，确保单例初始顺序
//         /// </summary>
//         public static void Init() => _ = Instance;
//         
//         public static T Instance
//         {
//             
//         }
//
//         private static T CreateInstance()
//         {
//             var go = new GameObject(typeof(T).Name);
//             var instance = go.AddComponent<T>();
//             _instance = instance;
//         }
//         
//     }
// }

