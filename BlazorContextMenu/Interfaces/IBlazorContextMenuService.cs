using System.Threading.Tasks;

namespace BlazorContextMenu;

public interface IBlazorContextMenuService
{
    /// <summary>
    /// Hides a <see cref="ContextMenu" /> programmatically.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task HideMenu(string id);

    /// <summary>
    /// Shows a <see cref="ContextMenu" /> programmatically.
    /// </summary>
    /// <param name="id">The id of the <see cref="ContextMenu"/>.</param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    Task ShowMenu(string id, int x, int y);

    /// <summary>
    /// Shows a <see cref="ContextMenu" /> programmatically.
    /// </summary>
    /// <param name="id">The id of the <see cref="ContextMenu"/>.</param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="data">Extra data that will be passed to menu events.</param>
    /// <returns></returns>
    Task ShowMenu(string id, int x, int y, object data);

    /// <summary>Determines if a <see cref="ContextMenu" /> is already being shown.</summary>
    /// <param name="id">The id of the <see cref="ContextMenu"/>.</param>
    /// <returns>True if the <see cref="ContextMenu" /> is being shown, otherwise False.</returns>
    Task<bool> IsMenuShown(string id);
}