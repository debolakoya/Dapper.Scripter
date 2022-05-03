using System.Data;
using System.Reflection;
using System.Text;

namespace ScriptBuilder.Builders;

public static class Builder
{
  private static Type? _type;
  private static string? _tableName;
  private static SortedDictionary<string, object?>? _parameters;
  private static Dictionary<string, object?>? _whereClauses;
  private static string? _identityFieldName;

  public static (IList<string>, Type) OfType<T>(string? tableName = null, string? identityFieldName = null) where T : class
  {
    _type = typeof(T);
    _identityFieldName = identityFieldName;
    _whereClauses= new Dictionary<string, object?>();
    _parameters = new SortedDictionary<string, object?>();
    _tableName = string.IsNullOrEmpty(tableName) ? typeof(T).Name : tableName;
    return new(new List<string>(), typeof(T));
  }

  public static (string, Dictionary<string, object>) Insert<T>(this (IEnumerable<string> fields, Type type) builder, T data) where T : class
  {
    if (typeof(T) != _type)
      throw new InvalidOperationException($"Data type ({typeof(T).Name}) does not match with the type of the builder ({_type?.Name}).");

    builder.fields.GenerateFieldValues(data);

    _parameters ??= new SortedDictionary<string, object?>();
    
    if (_parameters.Count == 0)
   throw new InvalidOperationException("No fields specified for the insert operation.");
    
    var scriptFields = new StringBuilder();
    scriptFields.Append($@"INSERT INTO {_tableName}({string.Join(", ", _parameters.Keys.Select(key => $"[{key}]"))}) VALUES ({string.Join(", ", _parameters.Keys.Select(key => $"@{key}"))})");

    var parameters = new Dictionary<string, object>();
    foreach (var (key, value) in _parameters)
      parameters.Add($"@{key}", BuilderExt.GetValue(value));

    if (!string.IsNullOrEmpty(_identityFieldName))
      scriptFields.Append(" SELECT SCOPE_IDENTITY();");

    return ($"{scriptFields}", parameters);
  }

  public static (string, Dictionary<string, object>) Update<TV>(this (IEnumerable<string> fields, Type type) builder, TV data) where TV : class
  {
    if (typeof(TV) != _type)
      throw new InvalidOperationException($"Data type ({typeof(TV).Name}) does not match with the type of the builder ({_type?.Name}).");

    builder.fields.GenerateFieldValues(data);

    _parameters ??= new SortedDictionary<string, object?>();

    if (_parameters.Count == 0)
   throw new InvalidOperationException("No fields specified for the update operation.");
    
    var scriptFields = new StringBuilder();
    scriptFields.Append($@"UPDATE {_tableName} SET {string.Join(", ", _parameters.Keys.Select(key => $"[{key}] = @{key}"))}");
    var parameters = new Dictionary<string, object>();

    foreach (var (key, value) in _parameters)
      parameters.Add($"@{key}", BuilderExt.GetValue(value));

    scriptFields = new StringBuilder(scriptFields.ToString().RemoveLastOccurrence(","));

    //if (keybuilderResult.keyValues.Count > 0)
    //{
    //    scriptFields.Append($" WHERE {keybuilderResult.clause}");

    //    foreach (var (key, value) in keybuilderResult.keyValues)
    //    {
    //        parameters.Add($"@{key}", StringHelper.GetValue(value));
    //    }

    //    scriptFields = new StringBuilder(scriptFields.ToString().RemoveLastOccurrence("AND"));
    //}


    return (scriptFields.ToString(), parameters);
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

  public static (IEnumerable<string>, Type) Where(this (IEnumerable<string> keyValues, Type type) builder, string field, Clause clause, object value)
  {
    if (_type == null)
      throw new InvalidOperationException("Builder is not initialized. Please call OfType method first.");

     var property = builder.type.GetProperty(field);

      if (property == null)
        throw new KeyNotFoundException($"{field} does not exist in {nameof(builder.type)}");
    
      _whereClauses??= new Dictionary<string, object?>();
    
    _whereClauses.Add (field , value );

    return builder ;
  }

  //public static (IList<string> keyValues) WhereAnd(this (IList<string> keyValues) keybuilderResult, string field, Clause clause, object value)
  //{
  //  throw new NotImplementedException();
  //}

  //public static (IList<string> keyValues) WhereBetween(this (IList<string> keyValues) keybuilderResult, string field, object value1, object value2)
  //{
  //  throw new NotImplementedException();
  //}

  //public static (IList<string> keyValues) WhereOr(this (IList<string> keyValues) keybuilderResult, string field, Clause clause, object value)
  //{
  //  throw new NotImplementedException();
  //}

  //public static (IList<string> keyValues) WhereIn(this (IList<string> keyValues) keybuilderResult, string field, IEnumerable<object> values)
  //{
  //  throw new NotImplementedException();
  //}

  //public static (IList<string> keyValues) WhereNotIn(this (IList<string> keyValues) keybuilderResult, string field, IEnumerable<object> values)
  //{
  //  throw new NotImplementedException();
  //}

  //public static (IList<string> keyValues) WhereLike(this (IList<string> keyValues) keybuilderResult, string field, object value)
  //{
  //  throw new NotImplementedException();
  //}

  //public static (IList<string> keyValues) WhereNotLike(this (IList<string> keyValues) keybuilderResult, string field, object value)
  //{
  //  throw new NotImplementedException();
  //}

  //public static (IList<string> keyValues) WhereNull(this (IList<string> keyValues) keybuilderResult, string field)
  //{
  //  throw new NotImplementedException();
  //}

  //public static (IList<string> keyValues) WhereNotNull(this (IList<string> keyValues) keybuilderResult, string field)
  //{
  //  throw new NotImplementedException();
  //}
}

