using System.Collections.Generic;
using System.Linq;

namespace BlazorContextMenu.Services;

public class MenuTreeTraverser : IMenuTreeTraverser
{
    #region Public Methods

    public ContextMenu GetRootContextMenu(MenuTreeComponent menuTreeComponent)
    {
        while (true)
        {
            switch (menuTreeComponent.ParentComponent)
            {
                case null:
                    return null;
                case ContextMenu contextMenu:
                    return contextMenu;
                default:
                    menuTreeComponent = menuTreeComponent.ParentComponent;
                    break;
            }
        }
    }

    public ContextMenuBase GetClosestContextMenu(MenuTreeComponent menuTreeComponent)
    {
        return menuTreeComponent.ParentComponent switch
        {
            null                     => null,
            ContextMenuBase menuBase => menuBase,
            _                        => GetClosestContextMenu(menuTreeComponent.ParentComponent)
        };
    }

    public bool HasSubMenu(MenuTreeComponent menuTreeComponent)
    {
        IReadOnlyList<MenuTreeComponent> children = menuTreeComponent.GetChildComponents();
        return children.Any(x => x is SubMenu) || children.Any(HasSubMenu);
    }

    #endregion
}