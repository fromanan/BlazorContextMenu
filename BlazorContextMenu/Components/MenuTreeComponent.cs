using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace BlazorContextMenu;

public abstract class MenuTreeComponent : ComponentBase, IDisposable
{
    [CascadingParameter(Name = "ParentComponent")]
    public MenuTreeComponent ParentComponent { get; protected set; }
    protected List<MenuTreeComponent> _childComponents = new List<MenuTreeComponent>();

    public IReadOnlyList<MenuTreeComponent> GetChildComponents()
    {
        return ChildComponents.AsReadOnly();
    }

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

    public virtual void Dispose()
    {
        GC.SuppressFinalize(this);
        ParentComponent?.RemoveChild(this);
    }
}