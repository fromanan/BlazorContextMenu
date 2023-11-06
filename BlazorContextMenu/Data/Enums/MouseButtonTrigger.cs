using System;

namespace BlazorContextMenu;

[Flags]
public enum MouseButtonTrigger
{
    None        = 0,
    Right       = 1 << 0,
    Left        = 1 << 1,
    Both        = Right | Left,
    DoubleClick = 1 << 2
}