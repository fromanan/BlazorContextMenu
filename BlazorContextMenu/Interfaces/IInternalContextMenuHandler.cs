using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace BlazorContextMenu;

public interface IInternalContextMenuHandler
{
    bool ReferencePassedToJs { get; set; }
    
    Task<bool> HideMenu(string id);

    Task ShowMenu(string id, string x, string y, string targetId = null,
        DotNetObjectReference<ContextMenuTrigger> trigger = null);
}