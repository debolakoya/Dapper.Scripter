using System.Data;
using System.Reflection;
using System.Text;
using ScriptBuilder;

namespace SQLScriptHelper;

public class ScriptBuilder<T> : IScriptBuilder<T> where T : class
{
  private readonly string _tableName;
  private readonly string _identityFieldName;
  private readonly SortedDictionary<string, object?> _parameters = new();
  private readonly SortedDictionary<string, object?> _keys = new();

  public ScriptBuilder(string identityFieldName, string? tableName = null)
  {
    _tableName = tableName ?? typeof(T).Name;
    _identityFieldName = identityFieldName;
  }

  public (string, Dictionary<string, object>) GenerateInsert(T data, string[] fields)
  {
    GenerateFieldValues(data, fields);
    return InsertScript();
  }

  public (string, Dictionary<string, object>) GenerateUpdate(T data, string[] fields, (Dictionary<string,object> parameters, string clause) whereClause)
  {
    ValidateKeys(whereClause.parameters);
    GenerateFieldValues(data, fields);
    return UpdateScript(whereClause);
  }

  private static void ValidateKeys(Dictionary<string, object> parameters)
  {
    foreach (var item in parameters)
    {
      GetProperty(item.Key);
    }
  }

  private void GenerateFieldValues(T data, IEnumerable<string> fields)
  {
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
    var property = typeof(T).GetProperty(field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

    if (property == null)
      throw new KeyNotFoundException($"{field} does not exist in {typeof(T).Name} Object");
    return property;
  }

  private (string, Dictionary<string, object>) InsertScript()
  {
    var scriptFields = new StringBuilder();
    var scriptParameters = new StringBuilder();
    var parameters = new Dictionary<string, object>();
    scriptFields.Append($@"INSERT INTO {_tableName}(");
    scriptParameters.Append(@" VALUES (");

    foreach (var (key, value) in _parameters)
    {
      scriptFields.Append($"{key}, ");
      var mValue = GetValue(value);  
      scriptParameters.Append($"{mValue},");
      parameters.Add($"@{key}", mValue);
    }

    scriptFields = new StringBuilder(scriptFields.ToString().RemoveLastOccurrence(",")).Append(')');
    scriptParameters = new StringBuilder(scriptParameters.ToString().RemoveLastOccurrence(",")).Append(')');

    if (!string.IsNullOrEmpty(_identityFieldName))
      scriptParameters.Append(" SELECT SCOPE_IDENTITY();");

    return ($"{scriptFields}{scriptParameters}", parameters);
  }
 
  private (string, Dictionary<string, object>) UpdateScript((Dictionary<string,object> parameters, string clause) whereClause)
  {
    var scriptFields = new StringBuilder();
    var parameters = new Dictionary<string, object>();
    scriptFields.Append($@"UPDATE {_tableName} SET");

    foreach (var (key, value) in _parameters)
    {
      scriptFields.Append($"{key}= {ScriptBuilder<T>.GetValue(value)}, ");
      parameters.Add($"@{key}", ScriptBuilder<T>.GetValue(value));
    }

    scriptFields = new StringBuilder(scriptFields.ToString().RemoveLastOccurrence(","));

    if (whereClause.parameters?.Count> 0)
    {
      scriptFields.Append($" WHERE {whereClause.clause}");

      foreach (var (key, value) in whereClause.parameters)
      {
        parameters.Add($"@{key}", ScriptBuilder<T>.GetValue(value));
      }

      scriptFields = new StringBuilder(scriptFields.ToString().RemoveLastOccurrence("AND"));
    }
    

    return (scriptFields.ToString(), parameters);
  }

  private static object GetValue(object? value)
  {
    //TODO: add quote based on data type
    return value ?? "null";
  }
}