using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorContextMenu;
using BlazorContextMenu.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.CompilerServices;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;

namespace BlazorContextMenu;

public class ContextMenuTrigger : ComponentBase, IDisposable
{
    #region Injection

    [Inject]
    private IJSRuntime JsRuntime { get; set; }

    [Inject]
    private IInternalContextMenuHandler InternalContextMenuHandler { get; set; }

    #endregion

    #region Parameters

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> Attributes { get; set; }

    /// <summary>
    /// The id of the <see cref="ContextMenuTrigger" /> wrapper element.
    /// </summary>
    [Parameter]
    public string Id { get; set; }

    /// <summary>
    /// The Id of the <see cref="ContextMenu" /> to open. This parameter is required.
    /// </summary>
    [Parameter]
    public string MenuId { get; set; }

    /// <summary>
    /// Additional css class for the trigger's wrapper element.
    /// </summary>
    [Parameter]
    public string CssClass { get; set; }

    /// <summary>
    /// The mouse button that triggers the menu.
    ///
    /// </summary>
    [Parameter]
    public MouseButtonTrigger MouseButtonTrigger { get; set; }

    /// <summary>
    /// The trigger's wrapper element tag (default: "div").
    /// </summary>
    [Parameter]
    public string WrapperTag { get; set; } = "div";

    /// <summary>
    /// Extra data that will be passed to menu events.
    /// </summary>
    [Parameter]
    public object Data { get; set; }

    /// <summary>
    /// Set to false if you do not want the click event to stop propagating. Default: true
    /// </summary>
    [Parameter]
    public bool StopPropagation { get; set; } = true;

    [Parameter]
    public RenderFragment ChildContent { get; set; }

    #endregion

    #region Data Members

    private ElementReference? _contextMenuTriggerElementRef;
    
    private DotNetObjectReference<ContextMenuTrigger> _dotNetObjectRef;

    #endregion

    #region Properties


    #endregion
    
    #region Protected Methods

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, WrapperTag);

        builder.AddMultipleAttributes(1, RuntimeHelpers.TypeCheck<IEnumerable<KeyValuePair<string, object>>>(Attributes));

        if (MouseButtonTrigger.HasFlag(MouseButtonTrigger.Left) || MouseButtonTrigger.HasFlag(MouseButtonTrigger.Both))
            builder.AddAttribute(2, "onclick", EventHandler);

        if (MouseButtonTrigger.HasFlag(MouseButtonTrigger.Right) || MouseButtonTrigger.HasFlag(MouseButtonTrigger.Both))
            builder.AddAttribute(3, "oncontextmenu", EventHandler);

        if (MouseButtonTrigger.HasFlag(MouseButtonTrigger.DoubleClick))
            builder.AddAttribute(4, "ondblclick", EventHandler);

        if (!CssClass.IsNullOrWhiteSpace())
            builder.AddAttribute(5, "class", CssClass);

        builder.AddAttribute(6, "id", Id);
        builder.AddContent(7, ChildContent);
        builder.AddElementReferenceCapture(8, value => _contextMenuTriggerElementRef = value);
        builder.CloseElement();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!internalContextMenuHandler.ReferencePassedToJs)
        {
            await jsRuntime.InvokeAsync<object>("blazorContextMenu.SetMenuHandlerReference", DotNetObjectReference.Create(internalContextMenuHandler));
            internalContextMenuHandler.ReferencePassedToJs = true;
        }

        if (dotNetObjectRef == null)
        {
            dotNetObjectRef = DotNetObjectReference.Create(this);
        }

        if (contextMenuTriggerElementRef != null)
        {
            await jsRuntime.InvokeAsync<object>("blazorContextMenu.RegisterTriggerReference", contextMenuTriggerElementRef.Value, dotNetObjectRef);
        }
    }

    #endregion

    #region IDisposable Implementation

    public void Dispose()
    {
        if (dotNetObjectRef != null)
        {
            dotNetObjectRef.Dispose();
            dotNetObjectRef = null;
        }
    }

    #endregion
}