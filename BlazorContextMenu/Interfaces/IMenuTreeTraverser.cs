namespace BlazorContextMenu;

public interface IMenuTreeTraverser
{
    ContextMenuBase GetClosestContextMenu(MenuTreeComponent menuTreeComponent);
    
    ContextMenu GetRootContextMenu(MenuTreeComponent menuTreeComponent);
    
    bool HasSubMenu(MenuTreeComponent menuTreeComponent);
}