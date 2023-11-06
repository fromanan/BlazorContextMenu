using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace BlazorContextMenu.Services;

public class BlazorContextMenuService : IBlazorContextMenuService
{
    #region Data Members

    private readonly IJSRuntime _jSRuntime;
    
    private readonly IContextMenuStorage _contextMenuStorage;

    #endregion

    #region Constructor
    
    public BlazorContextMenuService(IJSRuntime jSRuntime, IContextMenuStorage contextMenuStorage)
    {
        _jSRuntime = jSRuntime;
        _contextMenuStorage = contextMenuStorage;
    }

    #endregion

    #region Public Methods

    public async Task HideMenu(string id)
    {
        if (_contextMenuStorage.GetMenu(id) is null)
            throw new Exception($"No context menu with id '{id}' was found");

        await _jSRuntime.InvokeVoidAsync("blazorContextMenu.Hide", id);
    }

    public async Task ShowMenu(string id, int x, int y, object data)
    {
        if (_contextMenuStorage.GetMenu(id) is not { } menu)
            throw new Exception($"No context menu with id '{id}' was found");

        menu.Data = data;

        await _jSRuntime.InvokeVoidAsync("blazorContextMenu.ManualShow", id, x, y);
    }

    public Task ShowMenu(string id, int x, int y)
    {
        return ShowMenu(id, x, y, null);
    }

    public async Task<bool> IsMenuShown(string id) =>
        await _jSRuntime.InvokeAsync<bool>("blazorContextMenu.IsMenuShown", id);

    #endregion
}