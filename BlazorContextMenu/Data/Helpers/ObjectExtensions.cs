namespace BlazorContextMenu;

internal static class ObjectExtensions
{
    public static string ToWeb(this object obj)
    {
        return obj.ToString()?.ToLower();
    }
}