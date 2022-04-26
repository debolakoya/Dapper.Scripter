using System.Text;

namespace ScriptBuilder;

public static class StringHelper
{
    public static string RemoveLastOccurrence(this string currentString, string valueToRemove, string replaceWith = "")
    {
        var index = currentString.LastIndexOf(valueToRemove, StringComparison.OrdinalIgnoreCase);
        return index == -1
            ? currentString
            : currentString.Remove(index, valueToRemove.Length).Insert(index, replaceWith);
    }

    public static string GetValue(object? value)
    {
        return value switch
        {
            null => "null",
            bool => $"{value.ToString()?.ToLower()}",
            DateTime dateValue => $"'{dateValue:yyyy-MM-dd HH:mm:ss}'",
            DateOnly dateValue => $"'{dateValue:yyyy-MM-dd}'",
            TimeOnly dateValue => $"'{dateValue:HH:mm:ss}'",
            decimal => $"{value}",
            double => $"{value}",
            float => $"{value}",
            int => $"{value}",
            long => $"{value}",
            byte[] bytes => $"'{Encoding.UTF8.GetString(bytes)}'",
            _ => $"'{value}'"
        };
    }
}