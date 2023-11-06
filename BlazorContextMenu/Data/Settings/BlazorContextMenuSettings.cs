using System;
using System.Collections.Generic;

namespace BlazorContextMenu;

public class BlazorContextMenuSettings
{
    public const string DEFAULT_TEMPLATE_NAME = "default_{89930AFB-8CC8-4672-80D1-EA8BBE65B52A}";

    public readonly Dictionary<string, BlazorContextMenuTemplate> Templates = new()
    {
        { DEFAULT_TEMPLATE_NAME, new BlazorContextMenuTemplate() }
    };

    public BlazorContextMenuTemplate GetTemplate(string templateName)
    {
        if (!Templates.ContainsKey(templateName))
            throw new Exception($"Template '{templateName}' not found");
        return Templates[templateName];
    }
}