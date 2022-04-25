namespace SQLScriptHelper;

public static class FieldBuilder
{
    private static Type? _type ;

    public static IList<string> OfType<T>() where T : class
    {
        _type = typeof(T);
        return new List<string>();
    }

    public static IEnumerable<string> AddField(this IList<string> fields, string field)
    {
        if (_type == null)
            throw new  InvalidOperationException($"FieldBuilder is not initialized. Please call OfType method first.");

        var property = _type.GetProperty(field);

        if (property == null)
            throw new KeyNotFoundException($"{field} does not exist in {nameof(_type)}");

        fields.Add(property.Name);
        return fields;
    }
 

    public static IEnumerable<string> AddFields(this IEnumerable<string> fields)
    {
        if (_type == null)
            throw new  InvalidOperationException($"FieldBuilder is not initialized. Please call OfType method first.");
        
        var properties = _type.GetProperties();
        return fields.Concat(properties.Select(property => property.Name));
    }

    public static IEnumerable<string> Except(this IEnumerable<string> fields, string field)
    {
        if (_type == null)
            throw new  InvalidOperationException($"FieldBuilder is not initialized. Please call OfType method first.");

        var property = _type.GetProperty(field);

        if (property == null)
            throw new KeyNotFoundException($"{field} does not exist in {nameof(_type)}");

        var fieldList = fields.ToList();
        if (fieldList.Contains(field))
            fieldList.Remove(property.Name);

        return fieldList;
    }
}