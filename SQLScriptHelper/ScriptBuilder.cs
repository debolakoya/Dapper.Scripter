using System.Data;
using System.Reflection;
using System.Text;

namespace SQLScriptHelper;

public class ScriptBuilder<T> : IScriptBuilder<T> where T : class
{
    private readonly string _tableName;
    private readonly string _identityFieldName;
    private readonly SortedDictionary<string, object?> _parameters = new();

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

    private void GenerateFieldValues(T data, IEnumerable<string> fields)
    {
        foreach (var field in fields)
        {
            if (_parameters.ContainsKey(field))
                throw new DuplicateNameException($"{field} was already declared");

            var property = typeof(T).GetProperty(field, BindingFlags.IgnoreCase |  BindingFlags.Public | BindingFlags.Instance);

            if (property == null)
                throw new KeyNotFoundException($"{field} does not exist in {typeof(T).Name} Object");

            if (!string.Equals(property.Name, _identityFieldName, StringComparison.CurrentCultureIgnoreCase))
                _parameters.Add(field, property.GetValue(data));
        }
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
           // var mvalue = GetValue(value); TODO:
            scriptParameters.Append($"{value ?? "null"},");
            parameters.Add($"@{key}", value ?? "null");
        }

        scriptFields = new StringBuilder(scriptFields.ToString().RemoveLastOccurrence(",")).Append(')');
        scriptParameters = new StringBuilder(scriptParameters.ToString().RemoveLastOccurrence(",")).Append(')');

        if (!string.IsNullOrEmpty(_identityFieldName))
            scriptParameters.Append(" SELECT SCOPE_IDENTITY();");

        return ($"{scriptFields}{scriptParameters}", parameters);
    }
 }