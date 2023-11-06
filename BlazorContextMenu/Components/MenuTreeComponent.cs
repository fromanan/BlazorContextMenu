using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace BlazorContextMenu;

public abstract class MenuTreeComponent : ComponentBase, IDisposable
{
    #region Parameters

    [CascadingParameter(Name = "ParentComponent")]
    public MenuTreeComponent ParentComponent { get; protected set; }

    #endregion

    #region Data Members

    protected readonly List<MenuTreeComponent> ChildComponents = new();

    #endregion

    #region Public Methods

    public IReadOnlyList<MenuTreeComponent> GetChildComponents()
    {
        return ChildComponents.AsReadOnly();
    }

    #endregion

    #region Protected Methods

    protected void RegisterChild(MenuTreeComponent childComponent)
    {
        ChildComponents.Add(childComponent);
    }

    protected void RemoveChild(MenuTreeComponent childComponent)
    {
        ChildComponents.Remove(childComponent);
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (ParentComponent is null)
            return;
        ParentComponent.RegisterChild(this);
        ParentComponent.StateHasChanged();
    }

    #endregion

    #region IDisposable Implementation

    public virtual void Dispose()
    {
        GC.SuppressFinalize(this);
        ParentComponent?.RemoveChild(this);
    }

    #endregion
}