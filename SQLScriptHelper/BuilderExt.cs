using System.Text;

namespace ScriptBuilder.Builders
{
  public static class BuilderExt
  {
    public static (IEnumerable<string>, Type) AddField(this (IList<string> fields, Type type) builder, string field)
    {
      IsTypeInitialized(builder);
      var property = builder.type.GetProperty(field);

      if (property == null)
        throw new KeyNotFoundException($"{field} does not exist in {builder.type.Name}");

      builder.fields.Add(property.Name);
      return builder;
    }

    public static (IEnumerable<string>, Type) AddFields(this (IEnumerable<string> fields, Type type) builder)
    {
      IsTypeInitialized(builder);
      var properties = builder.type.GetProperties();
      builder.fields = builder.fields.Concat(properties.Select(property => property.Name));
      return builder;
    }

    public static (IEnumerable<string>, Type) Except(this (IEnumerable<string> fields, Type type) builder, string field)
    {
      IsTypeInitialized(builder);
      var property = builder.type.GetProperty(field);

      if (property == null)
        throw new KeyNotFoundException($"{field} does not exist in {nameof(builder.type)}");

      var fieldList = builder.fields.ToList();
      if (fieldList.Contains(field))
        fieldList.Remove(property.Name);
      
      builder.fields = fieldList;
      return builder;
    }

    private static void IsTypeInitialized((IEnumerable<string> fields, Type type) builder)
    {
      if (builder.type == null)
        throw new InvalidOperationException("Builder is not initialized. Please call OfType method first.");
    }

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
}