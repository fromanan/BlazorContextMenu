using System.Collections.Generic;
using System.Linq;

namespace BlazorContextMenu.Services;

public class MenuTreeTraverser : IMenuTreeTraverser
{
    #region Public Methods

    public ContextMenu GetRootContextMenu(MenuTreeComponent menuTreeComponent)
    {
        if (menuTreeComponent.ParentComponent == null) return null;
        if (menuTreeComponent.ParentComponent.GetType() == typeof(ContextMenu)) return menuTreeComponent.ParentComponent as ContextMenu;
        return GetRootContextMenu(menuTreeComponent.ParentComponent);
    }

    public ContextMenuBase GetClosestContextMenu(MenuTreeComponent menuTreeComponent)
    {
        if (menuTreeComponent.ParentComponent == null) return null;
        if (typeof(ContextMenuBase).IsAssignableFrom(menuTreeComponent.ParentComponent.GetType())) return menuTreeComponent.ParentComponent as ContextMenuBase;
        return GetClosestContextMenu(menuTreeComponent.ParentComponent);
    }

    public bool HasSubMenu(MenuTreeComponent menuTreeComponent)
    {
        var children = menuTreeComponent.GetChildComponents();
        if (children.Any(x => x is SubMenu)) return true;
        foreach (var child in children)
        {
            if (HasSubMenu(child)) return true;
        }

        return false;
    }

    #endregion
}