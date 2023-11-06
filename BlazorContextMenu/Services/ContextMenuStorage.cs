using System.Collections.Generic;

namespace BlazorContextMenu.Services;

public interface IContextMenuStorage
{
    ContextMenuBase GetMenu(string id);
    void Register(ContextMenuBase menu);
    void Unregister(ContextMenuBase menu);
}

public class ContextMenuStorage : IContextMenuStorage
{
    #region Data Members

    private readonly Dictionary<string, ContextMenuBase> _initializedMenus = new();

    #endregion

    #region Public Methods

    public void Register(ContextMenuBase menu)
    {
        _initializedMenus[menu.Id] = menu;
    }
    public void Unregister(ContextMenuBase menu)
    {
        _initializedMenus.Remove(menu.Id);
    }

    public ContextMenuBase GetMenu(string id)
    {
        if (_initializedMenus.ContainsKey(id))
        {
            return _initializedMenus[id];
        }

        return null;
    }

    #endregion
}