using System.Reflection;
using System.Text;

namespace ScriptBuilder.Builders;

public static class UpdateBuilder
{
    private static Type? _type;
    private static string? _tableName;
    private static List<string>? _fields;
    private static SortedDictionary<string, object?>? _parameters;
    private static string? _identityFieldName;

    private static PropertyInfo GetProperty(string field)
    {
        var property = _type?.GetProperty(field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

        if (property == null)
            throw new KeyNotFoundException($"{field} does not exist in {_type?.Name} Object");

        return property;
    }

    public static (Dictionary<string, object> keyValues, StringBuilder clause) OfType<T>(string? tableName = null, string? identityFieldName = null) where T : class
    {
        _type = typeof(T);
        _fields = new List<string>();
        _identityFieldName = identityFieldName;
        _parameters = new SortedDictionary<string, object?>();
        _tableName = string.IsNullOrEmpty(tableName) ? typeof(T).Name : tableName;
        return new ValueTuple<Dictionary<string, object>, StringBuilder>(new Dictionary<string, object>(), new StringBuilder());
    }

    public static (Dictionary<string, object> keyValues, StringBuilder clause) AddField(this (Dictionary<string, object> keyValues, StringBuilder clause) keyValues, string field)
    {
        if (_type == null)
            throw new InvalidOperationException("UpdateBuilder is not initialized. Please call OfType method first.");

        var fieldName = GetProperty(field).Name;

        _fields?.Add(fieldName);
        return keyValues;
    }

    public static (Dictionary<string, object> keyValues, StringBuilder clause) AddFields(this (Dictionary<string, object> keyValues, StringBuilder clause) keyValues)
    {
        if (_type == null)
            throw new InvalidOperationException("UpdateBuilder is not initialized. Please call OfType method first.");

        var properties = _type.GetProperties();
        _fields?.AddRange(properties.Select(property => property.Name));
        return keyValues;
    }

    public static (Dictionary<string, object> keyValues, StringBuilder clause) Except(this (Dictionary<string, object> keyValues, StringBuilder clause) keybuilderResult, string field)
    {
        if (_type == null)
            throw new InvalidOperationException("UpdateBuilder is not initialized. Please call OfType method first.");

        var fieldName = GetProperty(field).Name;

        if (_fields != null && _fields.Contains(field))
            _fields.Remove(fieldName);

        return keybuilderResult;
    }

    public static (Dictionary<string, object> keyValues, StringBuilder clause) Where(this (Dictionary<string, object> keyValues, StringBuilder clause) keybuilderResult, string field, Clause clause, object value)
    {
        if (_type == null)
            throw new InvalidOperationException("UpdateBuilder is not initialized. Please call OfType method first.");

        GetProperty(field);

        if (!keybuilderResult.keyValues.Keys.Contains(field, StringComparer.OrdinalIgnoreCase))
            keybuilderResult.keyValues.Add(field, value);

        return keybuilderResult;
    }

    public static (Dictionary<string, object> keyValues, StringBuilder clause) WhereAnd(this (Dictionary<string, object> keyValues, StringBuilder clause) keybuilderResult, string field, Clause clause, object value)
    {
        throw new NotImplementedException();
    }

    public static (Dictionary<string, object> keyValues, StringBuilder clause) WhereBetween(this (Dictionary<string, object> keyValues, StringBuilder clause) keybuilderResult, string field, object value1, object value2)
    {
        throw new NotImplementedException();
    }

    public static (Dictionary<string, object> keyValues, StringBuilder clause) WhereOr(this (Dictionary<string, object> keyValues, StringBuilder clause) keybuilderResult, string field, Clause clause, object value)
    {
        throw new NotImplementedException();
    }

    public static (Dictionary<string, object> keyValues, StringBuilder clause) WhereIn(this (Dictionary<string, object> keyValues, StringBuilder clause) keybuilderResult, string field, IEnumerable<object> values)
    {
        throw new NotImplementedException();
    }

    public static (Dictionary<string, object> keyValues, StringBuilder clause) WhereNotIn(this (Dictionary<string, object> keyValues, StringBuilder clause) keybuilderResult, string field, IEnumerable<object> values)
    {
        throw new NotImplementedException();
    }

    public static (Dictionary<string, object> keyValues, StringBuilder clause) WhereLike(this (Dictionary<string, object> keyValues, StringBuilder clause) keybuilderResult, string field, object value)
    {
        throw new NotImplementedException();
    }

    public static (Dictionary<string, object> keyValues, StringBuilder clause) WhereNotLike(this (Dictionary<string, object> keyValues, StringBuilder clause) keybuilderResult, string field, object value)
    {
        throw new NotImplementedException();
    }

    public static (Dictionary<string, object> keyValues, StringBuilder clause) WhereNull(this (Dictionary<string, object> keyValues, StringBuilder clause) keybuilderResult, string field)
    {
        throw new NotImplementedException();
    }

    public static (Dictionary<string, object> keyValues, StringBuilder clause) WhereNotNull(this (Dictionary<string, object> keyValues, StringBuilder clause) keybuilderResult, string field)
    {
        throw new NotImplementedException();
    }

    public static (string, Dictionary<string, object>) Build<TV>(this (Dictionary<string, object> keyValues, StringBuilder clause) keybuilderResult, TV data) where TV : class
    {
        if (typeof(TV) != _type)
            throw new InvalidOperationException($"Data type ({typeof(TV).Name}) does not match with the type of the builder ({_type?.Name}).");

        var scriptFields = new StringBuilder();
        var parameters = new Dictionary<string, object>();
        _parameters ??= new SortedDictionary<string, object?>();
        scriptFields.Append($@"UPDATE {_tableName} SET");

        foreach (var (key, value) in _parameters)
        {
            scriptFields.Append($"{key}= {StringHelper.GetValue(value)}, ");
            parameters.Add($"@{key}", StringHelper.GetValue(value));
        }

        scriptFields = new StringBuilder(scriptFields.ToString().RemoveLastOccurrence(","));

        if (keybuilderResult.keyValues.Count > 0)
        {
            scriptFields.Append($" WHERE {keybuilderResult.clause}");

            foreach (var (key, value) in keybuilderResult.keyValues)
            {
                parameters.Add($"@{key}", StringHelper.GetValue(value));
            }

            scriptFields = new StringBuilder(scriptFields.ToString().RemoveLastOccurrence("AND"));
        }


        return (scriptFields.ToString(), parameters);
    }

}

