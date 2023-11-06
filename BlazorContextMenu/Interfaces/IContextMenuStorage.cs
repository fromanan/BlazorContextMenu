namespace BlazorContextMenu;

public interface IContextMenuStorage
{
    ContextMenuBase GetMenu(string id);
    
    void Register(ContextMenuBase menu);
    
    void Unregister(ContextMenuBase menu);
}