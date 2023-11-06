using System.Collections.Generic;

namespace BlazorContextMenu.Services;

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
        return _initializedMenus.TryGetValue(id, out ContextMenuBase menu) ? menu : null;
    }

    #endregion
}