using UnityEngine;

namespace Foli
{
    public interface IUIData { }

    public abstract class UIBase : MonoBehaviour
    {
        private bool _initialized;
        protected IUIData Data { get; private set; }

        private void InitInternal()
        {
            if (_initialized) return;
            OnInit();
            _initialized = true;
        }

        public void SetData(IUIData data)
        {
            Data = data;
            OnDataSet();
        }

        public void Refresh()
        {
            InitInternal();
            if (Data == null) return;
            OnRefresh();
        }

        public void SetVisible(bool visible)
        {
            if (gameObject.activeSelf == visible) return;

            if (visible) OnBeforeShow();
            else OnBeforeHide();

            gameObject.SetActive(visible);

            if (visible) OnShow();
            else OnHide();
        }

        protected abstract void OnInit();

        protected virtual void OnDataSet()
        {
        }

        protected abstract void OnRefresh();

        protected virtual void OnBeforeShow()
        {
        }

        protected virtual void OnBeforeHide()
        {
        }

        protected virtual void OnShow()
        {
        }

        protected virtual void OnHide()
        {
        }

        protected virtual void Awake()
        {
            InitInternal();
        }
    }
}

