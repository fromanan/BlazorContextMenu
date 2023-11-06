namespace BlazorContextMenu;

internal static class StringExtensions
{
    public static bool IsNullOrEmpty(this string str)
    {
        return string.IsNullOrEmpty(str);
    }
    
    public static bool IsNullOrWhiteSpace(this string str)
    {
        return string.IsNullOrWhiteSpace(str);
    }
    
    public static bool HasValue(this string str)
    {
        return !str.IsNullOrEmpty();
    }
}