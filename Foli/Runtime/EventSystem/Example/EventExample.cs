using UnityEngine;
using E = Foli.EventExample;
namespace Foli
{
    public static class EventExample
    {
        // 事件定义
        public class EventA
        {
            public int Argument1 = 1;
            public int Argument2 = 2;
            public int Argument3 = 3;
        }
        public class EventB
        {
            public int Argument1;
            public float Argument2;
            public string Argument3;
        }
        public class EventC { }

        // 事件域定义
        public static readonly EventDomain Domain = new();
    }


    public class ListenerExampleA : MonoBehaviour
    {
        // 注册事件
        private void Awake()
        {
            E.Domain.Subscribe<E.EventA>(OnEventA);
            E.Domain.Subscribe<E.EventB>(OnEventB);
            E.Domain.Subscribe<E.EventC>(OnEventC);
        }

        // 注销事件
        private void OnDestroy()
        {
            E.Domain.Unsubscribe<E.EventA>(OnEventA);
            E.Domain.Unsubscribe<E.EventB>(OnEventB);
            E.Domain.Unsubscribe<E.EventC>(OnEventC);
        }

        // 事件回调
        private void OnEventA(E.EventA e) { }
        private void OnEventB(E.EventB e) { }
        private void OnEventC(E.EventC e) { }
    }
    
    public class ListenerExampleB : MonoBehaviour
    {
        private void Awake()
        {
            E.Domain.Subscribe<E.EventA>(OnEventA, 1); // 比 ListenerExampleA 更先执行回调
        }
        private void OnDestroy()
        {
            E.Domain.Unsubscribe<E.EventA>(OnEventA);
        }
        private void OnEventA(E.EventA e) { }
    }

    public class CallerExampleB
    {
        // 发布事件
        private void Call()
        {
            E.Domain.Publish(new E.EventA());
            E.Domain.Publish(new E.EventB { Argument1 = 4, Argument2 = 5, Argument3 = "6" });
            E.Domain.Publish(new E.EventC());
        }
    }
}