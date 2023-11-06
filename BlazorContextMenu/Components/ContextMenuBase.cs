using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorContextMenu;

public abstract class ContextMenuBase : MenuTreeComponent
{
    #region Dependency Injection

    [Inject]
    private BlazorContextMenuSettings Settings { get; set; }

    [Inject]
    private IContextMenuStorage ContextMenuStorage { get; set; }

    [Inject]
    private IInternalContextMenuHandler ContextMenuHandler { get; set; }

    [Inject]
    private IJSRuntime JsRuntime { get; set; }

    [Inject]
    private IMenuTreeTraverser MenuTreeTraverser { get; set; }

    #endregion

    #region Parameters

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> Attributes { get; set; }

    protected virtual string BaseClass => "blazor-context-menu blazor-context-menu__wrapper";

    /// <summary>
    /// The id that the <see cref="ContextMenuTrigger" /> will use to bind to. This parameter is required
    /// </summary>
    [Parameter]
    public required string Id { get; set; }

    /// <summary>
    /// The name of the template to use for this <see cref="ContextMenu" /> and all its <see cref="SubMenu" />.
    /// </summary>
    [Parameter]
    public string Template { get; set; }

    [CascadingParameter(Name = "CascadingTemplate")]
    protected string CascadingTemplate { get; set; }

    /// <summary>
    /// Allows you to override the default css class of the <see cref="ContextMenu"/>'s div element, for full customization.
    /// </summary>
    [Parameter]
    public string OverrideDefaultCssClass { get; set; }

    /// <summary>
    /// Allows you to override the default css class of the <see cref="ContextMenu"/>'s div element while it's shown, for full customization.
    /// </summary>
    [Parameter]
    public string OverrideDefaultShownCssClass { get; set; }

    /// <summary>
    /// Allows you to override the default css class of the <see cref="ContextMenu"/>'s div element while it's hidden, for full customization.
    /// </summary>
    [Parameter]
    public string OverrideDefaultHiddenCssClass { get; set; }

    /// <summary>
    /// Allows you to override the default css class of the <see cref="ContextMenu"/>'s ul element, for full customization.
    /// </summary>
    [Parameter]
    public string OverrideDefaultListCssClass { get; set; }

    /// <summary>
    /// Additional css class that is applied to the <see cref="ContextMenu"/>'s div element. Use this to extend the default css.
    /// </summary>
    [Parameter]
    public string CssClass { get; set; }

    /// <summary>
    /// Additional css class that is applied to the <see cref="ContextMenu"/>'s div element while is shown. Use this to extend the default css.
    /// </summary>
    [Parameter]
    public string ShownCssClass { get; set; }

    /// <summary>
    /// Additional css class that is applied to the <see cref="ContextMenu"/>'s div element while is hidden. Use this to extend the default css.
    /// </summary>
    [Parameter]
    public string HiddenCssClass { get; set; }

    /// <summary>
    /// Additional css class that is applied to the <see cref="ContextMenu"/>'s ul element. Use this to extend the default css.
    /// </summary>
    [Parameter]
    public string ListCssClass { get; set; }

    /// <summary>
    /// Allows you to set the <see cref="BlazorContextMenu.Animation" /> used by this <see cref="ContextMenu" /> and all its <see cref="SubMenu" />
    /// </summary>
    [Parameter]
    public Animation? Animation { get; set; }

    /// <summary>
    /// A handler that is triggered before the menu appears. Can be used to prevent the menu from showing.
    /// </summary>
    [Parameter]
    public EventCallback<MenuAppearingEventArgs> OnAppearing { get; set; }

    /// <summary>
    /// A handler that is triggered before the menu is hidden. Can be used to prevent the menu from hiding.
    /// </summary>
    [Parameter]
    public EventCallback<MenuHidingEventArgs> OnHiding { get; set; }

    /// <summary>
    /// Set to false if you want to close the menu programmatically. Default: true
    /// </summary>
    [Parameter]
    public bool AutoHide { get; set; } = true;

    /// <summary>
    /// Set to AutoHideEvent.MouseUp if you want it to close the menu on the MouseUp event. Default: AutoHideEvent.MouseDown
    /// </summary>
    [Parameter]
    public AutoHideEvent AutoHideEvent { get; set; } = AutoHideEvent.MouseDown;

    /// <summary>
    /// Set CSS z-index for overlapping other html elements. Default: 1000
    /// </summary>
    [Parameter]
    public int ZIndex { get; set; } = 1000;

    [CascadingParameter(Name = "CascadingAnimation")]
    protected Animation? CascadingAnimation { get; set; }

    #endregion

    #region Properties

    protected bool IsShowing;
    protected string X { get; set; }
    protected string Y { get; set; }
    protected string TargetId { get; set; }
    protected ContextMenuTrigger Trigger { get; set; }
    internal object Data { get; set; }

    protected string ClassCalc
    {
        get
        {
            BlazorContextMenuTemplate template = Settings.GetTemplate(GetActiveTemplate());
            return Helpers.AppendCssClasses(OverrideDefaultCssClass ?? template.DefaultCssOverrides.MenuCssClass,
                CssClass ?? template.MenuCssClass);
        }
    }
    
    protected string PositionStyles => $"left:{X}px;top:{Y}px;z-index:{ZIndex};";
    
    protected string Classes => $"{BaseClass} {ClassCalc} {DisplayClassCalc}";

    #endregion

    #region Protected Methods

    protected Animation GetActiveAnimation()
    {
        return Animation ?? CascadingAnimation ?? Settings.GetTemplate(GetActiveTemplate()).Animation;
    }

    protected string DisplayClassCalc
    {
        get
        {
            BlazorContextMenuTemplate template = Settings.GetTemplate(GetActiveTemplate());
            DisplayClassRecord animationClasses = GetAnimationClasses(GetActiveAnimation());
            return IsShowing
                ? Helpers.AppendCssClasses(
                    OverrideDefaultShownCssClass ?? template.DefaultCssOverrides.MenuShownCssClass,
                    animationClasses.VisibleClass,
                    ShownCssClass ?? Settings.GetTemplate(GetActiveTemplate()).MenuShownCssClass)
                : Helpers.AppendCssClasses(
                    OverrideDefaultHiddenCssClass ?? template.DefaultCssOverrides.MenuHiddenCssClass,
                    animationClasses.HiddenClass,
                    HiddenCssClass ?? Settings.GetTemplate(GetActiveTemplate()).MenuHiddenCssClass);
        }
    }

    protected string ListClassCalc
    {
        get
        {
            BlazorContextMenuTemplate template = Settings.GetTemplate(GetActiveTemplate());
            return Helpers.AppendCssClasses(
                OverrideDefaultListCssClass ?? template.DefaultCssOverrides.MenuListCssClass,
                ListCssClass ?? Settings.GetTemplate(GetActiveTemplate()).MenuListCssClass);
        }
    }

    protected record DisplayClassRecord(string VisibleClass = "", string HiddenClass = "");

    private const string _ANIMATIONS_CLASS = "blazor-context-menu__animations";

    protected static DisplayClassRecord GetAnimationClasses(Animation animation)
    {
        return animation switch
        {
            BlazorContextMenu.Animation.None => new DisplayClassRecord(),
            BlazorContextMenu.Animation.FadeIn => new DisplayClassRecord
            (
                $"{_ANIMATIONS_CLASS}--fadeIn-shown",
                $"{_ANIMATIONS_CLASS}--fadeIn"
            ),
            BlazorContextMenu.Animation.Grow => new DisplayClassRecord
            (
                $"{_ANIMATIONS_CLASS}--grow-shown",
                $"{_ANIMATIONS_CLASS}--grow"
            ),
            BlazorContextMenu.Animation.Slide => new DisplayClassRecord
            (
                $"{_ANIMATIONS_CLASS}--slide-shown",
                $"{_ANIMATIONS_CLASS}--slide"
            ),
            BlazorContextMenu.Animation.Zoom => new DisplayClassRecord
            (
                $"{_ANIMATIONS_CLASS}--zoom-shown",
                $"{_ANIMATIONS_CLASS}--zoom"
            ),
            _ => throw new Exception("Animation not supported")
        };
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        ContextMenuStorage.Register(this);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!ContextMenuHandler.ReferencePassedToJs)
        {
            await JsRuntime.InvokeAsync<object>("blazorContextMenu.SetMenuHandlerReference",
                DotNetObjectReference.Create(ContextMenuHandler));
            ContextMenuHandler.ReferencePassedToJs = true;
        }
    }

    #endregion

    #region Internal Methods

    internal string GetActiveTemplate()
    {
        return Template ?? CascadingTemplate ?? BlazorContextMenuSettings.DEFAULT_TEMPLATE_NAME;
    }
    
    internal async Task Show(string x, string y, string targetId = null, ContextMenuTrigger trigger = null)
    {
        if (trigger is null)
        {
            ContextMenu rootMenu = MenuTreeTraverser.GetRootContextMenu(this);
            trigger = rootMenu?.GetTrigger();
        }

        if (trigger is not null)
            Data = trigger.Data;

        if (OnAppearing.HasDelegate)
        {
            MenuAppearingEventArgs eventArgs = new(Id, targetId, x, y, trigger, Data);
            await OnAppearing.InvokeAsync(eventArgs);
                
            x = eventArgs.X;
            y = eventArgs.Y;
                
            if (eventArgs.PreventShow)
                return;
        }
        
        IsShowing = true;
        X = x;
        Y = y;
        
        TargetId = targetId;
        Trigger = trigger;
            
        await InvokeAsync(StateHasChanged);
    }

    internal async Task<bool> Hide()
    {
        if (OnHiding.HasDelegate)
        {
            MenuHidingEventArgs eventArgs = new(Id, TargetId, X, Y, Trigger, Data);
            await OnHiding.InvokeAsync(eventArgs);
            if (eventArgs.PreventHide)
                return false;
        }

        IsShowing = false;
        await InvokeAsync(StateHasChanged);
        return true;
    }

    internal string GetTarget()
    {
        return TargetId;
    }

    internal ContextMenuTrigger GetTrigger()
    {
        return Trigger;
    }

    #endregion
    
    #region IDisposable Implementation

    public override void Dispose()
    {
        GC.SuppressFinalize(this);
        base.Dispose();
        ContextMenuStorage.Unregister(this);
    }
    
    #endregion
}