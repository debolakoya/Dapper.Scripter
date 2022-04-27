using System.Data;
using System.Reflection;
using System.Text;

namespace ScriptBuilder.Builders;

public static class InsertBuilder
{
    private static Type? _type;
    private static string? _tableName;
    private static SortedDictionary<string, object?>? _parameters;
    private static string? _identityFieldName;

    public static IList<string> OfType<T>(string? tableName = null, string? identityFieldName = null) where T : class
    {
        _type = typeof(T);
        _identityFieldName = identityFieldName;
        _parameters = new SortedDictionary<string, object?>();
        _tableName = string.IsNullOrEmpty(tableName) ? typeof(T).Name : tableName;
        return new List<string>();
    }

    public static IEnumerable<string> AddField(this IList<string> fields, string field)
    {
        if (_type == null)
            throw new InvalidOperationException("FieldBuilder is not initialized. Please call OfType method first.");

        var property = _type.GetProperty(field);

        if (property == null)
            throw new KeyNotFoundException($"{field} does not exist in {_type.Name}");

        fields.Add(property.Name);
        return fields;
    }

    public static IEnumerable<string> AddFields(this IEnumerable<string> fields)
    {
        if (_type == null)
            throw new InvalidOperationException("FieldBuilder is not initialized. Please call OfType method first.");

        var properties = _type.GetProperties();
        return fields.Concat(properties.Select(property => property.Name));
    }

    public static IEnumerable<string> Except(this IEnumerable<string> fields, string field)
    {
        if (_type == null)
            throw new InvalidOperationException("FieldBuilder is not initialized. Please call OfType method first.");

        var property = _type.GetProperty(field);

        if (property == null)
            throw new KeyNotFoundException($"{field} does not exist in {nameof(_type)}");

        var fieldList = fields.ToList();
        if (fieldList.Contains(field))
            fieldList.Remove(property.Name);

        return fieldList;
    }

    public static (string, Dictionary<string, object>) Build<T>(this IEnumerable<string> fields, T data) where T : class
    {
        if (typeof(T) != _type)
            throw new InvalidOperationException($"Data type ({typeof(T).Name}) does not match with the type of the builder ({_type?.Name}).");

        fields.GenerateFieldValues(data);

        var scriptFields = new StringBuilder();
        _parameters ??= new SortedDictionary<string, object?>();
        scriptFields.Append($@"INSERT INTO {_tableName}({string.Join(", ", _parameters.Keys.Select(key => $"[{key}]"))}) VALUES ({string.Join(", ", _parameters.Keys.Select(key => $"@{key}"))})");

        var parameters = new Dictionary<string, object>();
        foreach (var (key, value) in _parameters)
            parameters.Add($"@{key}", StringHelper.GetValue(value));

        if (!string.IsNullOrEmpty(_identityFieldName))
            scriptFields.Append(" SELECT SCOPE_IDENTITY();");

        return ($"{scriptFields}", parameters);
    }

    private static void GenerateFieldValues<T>(this IEnumerable<string> fields, T data)
    {
        _parameters ??= new SortedDictionary<string, object?>();

        foreach (var field in fields)
        {
            if (_parameters.ContainsKey(field))
                throw new DuplicateNameException($"{field} was already declared");

            var property = GetProperty(field);

            if (!string.Equals(property.Name, _identityFieldName, StringComparison.CurrentCultureIgnoreCase))
                _parameters.Add(field, property.GetValue(data));
        }
    }

    private static PropertyInfo GetProperty(string field)
    {
        var property = _type?.GetProperty(field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

        if (property == null)
            throw new KeyNotFoundException($"{field} does not exist in {_type?.Name} Object");
        return property;
    }
}

