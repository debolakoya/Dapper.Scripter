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
}