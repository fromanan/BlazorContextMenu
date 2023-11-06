using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace BlazorContextMenu.Services;

public class InternalContextMenuHandler : IInternalContextMenuHandler
{
    #region Data Members

    private readonly IContextMenuStorage _contextMenuStorage;

    #endregion

    #region Properties

    public bool ReferencePassedToJs { get; set; } = false;

    #endregion

    #region Constructor

    public InternalContextMenuHandler(IContextMenuStorage contextMenuStorage)
    {
        _contextMenuStorage = contextMenuStorage;
    }

    #endregion

    #region JavaScript Methods

    /// <summary>
    /// Shows the context menu at the specified coordinates.
    /// </summary>
    /// <param name="id">The id of the menu.</param>
    /// <param name="x">The x coordinate on the screen.</param>
    /// <param name="y">The y coordinate on the screen.</param>
    /// <param name="targetId">Optional: The id of the element that triggered the menu show event.</param>
    /// <param name="trigger">Optional: The <see cref="ContextMenuTrigger"/> that opened the menu.</param>
    /// <returns></returns>
    [JSInvokable]
    public async Task ShowMenu(string id, string x, string y, string targetId = null,
        DotNetObjectReference<ContextMenuTrigger> trigger = null)
    {
        if (_contextMenuStorage.GetMenu(id) is { } menu)
            await menu.Show(x, y, targetId, trigger?.Value);
    }

    /// <summary>
    /// Hides a context menu
    /// </summary>
    /// <param name="id">The id of the menu.</param>
    [JSInvokable]
    public async Task<bool> HideMenu(string id)
    {
        return _contextMenuStorage.GetMenu(id) is not { } menu || await menu.Hide();
    }

    #endregion
}