using UnityEngine;

/// <summary> 更新依赖的数据 </summary>
public abstract class RegionBaseRefreshData { }

/// <summary> 显示依赖的参数 </summary>
public abstract class RegionBaseShowParam { }

public abstract class UIBase1 : MonoBehaviour
{
    #region 中介
    private UIHub _hub;
    public T GetHub<T>() where T : UIHub
    {
        return _hub as T;
    }
    public void SetHub(UIHub hub)
    {
        _hub = hub;
    }
    #endregion

    #region 外部调用
    private bool _isInit;
    public void Init()
    {
        if (_isInit) return;
        OnInit();
        _isInit = true;
    }
    public void Refresh(RegionBaseRefreshData data)
    {
        Init();
        OnRefresh(data);
    }
    public void Show(RegionBaseShowParam param)
    {
        if (gameObject.activeSelf) return;
        gameObject.SetActive(true);
        OnShow(param);
    }
    public void Hide()
    {
        if(!gameObject.activeSelf) return;
        OnHide();
        gameObject.SetActive(false);
    }
    #endregion

    #region 子类重写
    protected abstract void OnInit();
    protected abstract void OnRefresh(RegionBaseRefreshData data);
    protected virtual void OnShow(RegionBaseShowParam param) { }
    protected virtual void OnHide() { }
    #endregion
}