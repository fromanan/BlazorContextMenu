namespace BlazorContextMenu;

public class BlazorContextMenuDefaultCssSettings
{
    /// <summary>
    /// Allows you to override the default css class of the <see cref="ContextMenu"/>'s div element, for full customization.
    /// </summary>
    public string MenuCssClass { get; set; } = "blazor-context-menu--default";

    /// <summary>
    /// Allows you to override the default css class of the <see cref="ContextMenu"/>'s div element while it's shown, for full customization.
    /// </summary>
    public string MenuShownCssClass { get; set; } = "";

    /// <summary>
    /// Allows you to override the default css class of the <see cref="ContextMenu"/>'s div element while it's hidden, for full customization.
    /// </summary>
    public string MenuHiddenCssClass { get; set; } = "blazor-context-menu--hidden";

    /// <summary>
    /// Allows you to override the default css class of the <see cref="ContextMenu"/>'s ul element, for full customization.
    /// </summary>
    public string MenuListCssClass { get; set; } = "blazor-context-menu__list";

    /// <summary>
    /// Allows you to override the default css class of the menu <see cref="Item"/>'s li element, for full customization.
    /// </summary>
    public string MenuItemCssClass { get; set; } = "blazor-context-menu__item--default";

    /// <summary>
    /// Allows you to override the default css class of the menu <see cref="Item"/>'s li element when it contains a <see cref="SubMenu"/>, for full customization.
    /// </summary>
    public string MenuItemWithSubMenuCssClass { get; set; } = "blazor-context-menu__item--with-submenu";

    /// <summary>
    /// Allows you to override the default css class of the menu <see cref="Item"/>'s li element when disabled, for full customization.
    /// </summary>
    public string MenuItemDisabledCssClass { get; set; } = "blazor-context-menu__item--default-disabled";

    /// <summary>
    /// Allows you to override the default css class of the menu <see cref="Separator"/>'s li element, for full customization.
    /// </summary>
    public string SeparatorCssClass { get; set; } = "blazor-context-menu__separator";

    /// <summary>
    /// Allows you to override the default css class of the menu <see cref="Separator"/>'s hr element, for full customization.
    /// </summary>
    public string SeparatorHrCssClass { get; set; } = "blazor-context-menu__separator__hr";
}