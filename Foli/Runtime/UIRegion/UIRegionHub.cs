using System;
using System.Collections.Generic;
using UnityEngine;

public class UIRegionHub : MonoBehaviour
{
    #region 树状中介
    private readonly Dictionary<int, UIRegionBase> _regions = new();
    public void SetRegion(int id, UIRegionBase region)
    {
        if (region == null) return;
        _regions[id] = region;
        region.SetHub(this);
    }
    public T GetRegion<T>(int id) where T : UIRegionBase
    {
        if(_regions.TryGetValue(id, out var region))
            return region as T;
        return null;
    }
    #endregion

    #region 页面返回栈
    [Flags]
    public enum Options
    {
        None = 0,
        PushCurrent = 1 << 0,
        HideOthers = 1 << 1,
    }
    private readonly Stack<(int, RegionBaseShowParam)> _stack = new();
    private int _currentPage = -1;
    private RegionBaseShowParam _currentParam;
    
    public void SetPage(int page, RegionBaseShowParam param = null)
    {
        _currentPage = page;
        _currentParam = param;
    }
    public void GoPage(int page, RegionBaseShowParam param = null, Options options = Options.PushCurrent | Options.HideOthers)
    {
        if (!_regions.TryGetValue(page, out var region))
            return;
        if ((options & Options.HideOthers) != 0)
        {
            foreach (var r in _regions.Values)
                r.Hide();
        }
        if ((options & Options.PushCurrent) != 0)
        {
            if (_currentPage > 0 && page != _currentPage)
                _stack.Push((_currentPage, _currentParam));
        }
        
        SetPage(page, param);
        region.Show(param);
    }
    public void GoBack()
    {
        if (_stack.Count <= 0) return;
        var (page, param) = _stack.Pop();
        GoPage(page, param, Options.HideOthers);
    }
    #endregion
}