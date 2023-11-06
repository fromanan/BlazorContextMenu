namespace BlazorContextMenu;

public class BlazorContextMenuTemplate
{
    /// <summary>
    /// Additional css class that is applied to the <see cref="ContextMenu"/>'s div element. Use this to extend the default css.
    /// </summary>
    public string MenuCssClass { get; set; }

    /// <summary>
    /// Additional css class that is applied to the <see cref="ContextMenu"/>'s ul element. Use this to extend the default css.
    /// </summary>
    public string MenuListCssClass { get; set; }

    /// <summary>
    /// Additional css class for the menu <see cref="Item"/>'s li element. Use this to extend the default css.
    /// </summary>
    public string MenuItemCssClass { get; set; }

    /// <summary>
    /// Additional css class that is applied to the <see cref="ContextMenu"/>'s div element while it's shown. Use this to extend the default css.
    /// </summary>
    public string MenuShownCssClass { get; set; }

    /// <summary>
    /// Additional css class that is applied to the <see cref="ContextMenu"/>'s div element while it's hidden. Use this to extend the default css.
    /// </summary>
    public string MenuHiddenCssClass { get; set; }

    /// <summary>
    /// Additional css class for the menu <see cref="Item"/>'s li element when it contains a <see cref="SubMenu"/>. Use this to extend the default css.
    /// </summary>
    public string MenuItemWithSubMenuCssClass { get; set; }

    /// <summary>
    /// Additional css class for the menu <see cref="Item"/>'s li element when disabled. Use this to extend the default css.
    /// </summary>
    public string MenuItemDisabledCssClass { get; set; }

    /// <summary>
    /// Additional css class for the menu <see cref="Separator"/>'s li element. Use this to extend the default css.
    /// </summary>
    public string SeparatorCssClass { get; set; }

    /// <summary>
    /// Additional css class for the menu <see cref="Separator"/>'s hr element. Use this to extend the default css.
    /// </summary>
    public string SeparatorHrCssClass { get; set; }

    /// <summary>
    /// Allows you to override the default x position offset of the submenu (i.e. the distance of the submenu from it's parent menu).
    /// </summary>
    public int SubMenuXPositionPixelsOffset { get; set; } = 4;

    /// <summary>
    /// Allows you to set the <see cref="BlazorContextMenu.Animation" /> used by the <see cref="ContextMenu" />.
    /// </summary>
    public Animation Animation { get; set; }

    /// <summary>
    /// Exposes overrides to the default css classes for complete customization. Only recommended if you cannot 
    /// achieve customization otherwise and you must replace the default classes.
    /// </summary>
    public BlazorContextMenuDefaultCssSettings DefaultCssOverrides { get; set; } = new();
}