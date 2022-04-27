//using System.Data;
//using System.Reflection;
//using System.Text;

//namespace ScriptBuilder.Builders;

//public class InserterBuilder<T> where T : class
//{
//    private readonly Type? _type;
//    private readonly string? _tableName;
//    private readonly string? _identityFieldName;
//    private readonly List<string> _fields;
//    private SortedDictionary<string, object?>? _parameters;

//    public InserterBuilder(string? tableName = null, string? identityFieldName = null)
//    {
//        _type = typeof(T);
//        _identityFieldName = identityFieldName;
//        _parameters = new SortedDictionary<string, object?>();
//        _tableName = string.IsNullOrEmpty(tableName) ? typeof(T).Name : tableName;
//        _fields = new List<string>();
//    }

//    public IEnumerable<string> AddField(string field)
//    {
//        if (_type == null)
//            throw new InvalidOperationException("FieldBuilder is not initialized. Please call OfType method first.");

//        var property = _type.GetProperty(field);

//        if (property == null)
//            throw new KeyNotFoundException($"{field} does not exist in {_type.Name}");

//        _fields.Add(property.Name);
//        return _fields;
//    }

//    public IEnumerable<string> AddFields()
//    {
//        if (_type == null)
//            throw new InvalidOperationException("FieldBuilder is not initialized. Please call OfType method first.");

//        var properties = _type.GetProperties();
//        return _fields.Concat(properties.Select(property => property.Name));
//    }

//    public IEnumerable<string> Except(string field)
//    {
//        if (_type == null)
//            throw new InvalidOperationException("FieldBuilder is not initialized. Please call OfType method first.");

//        var property = _type.GetProperty(field);

//        if (property == null)
//            throw new KeyNotFoundException($"{field} does not exist in {nameof(_type)}");

//        var fieldList = _fields.ToList();
//        if (fieldList.Contains(field))
//            fieldList.Remove(property.Name);

//        return fieldList;
//    }

//    public (string, Dictionary<string, object>) Build<TV>(TV data) where TV : T
//    {
//        GenerateFieldValues(data);
//        return InsertScript();
//    }

//    private void GenerateFieldValues(T data)
//    {
//        _parameters ??= new SortedDictionary<string, object?>();

//        foreach (var field in _fields)
//        {
//            if (_parameters.ContainsKey(field))
//                throw new DuplicateNameException($"{field} was already declared");

//            var property = GetProperty(field);

//            if (!string.Equals(property.Name, _identityFieldName, StringComparison.CurrentCultureIgnoreCase))
//                _parameters.Add(field, property.GetValue(data));
//        }
//    }

//    private PropertyInfo GetProperty(string field)
//    {
//        var property = _type?.GetProperty(field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

//        if (property == null)
//            throw new KeyNotFoundException($"{field} does not exist in {_type?.Name} Object");
//        return property;
//    }

//    private (string, Dictionary<string, object>) InsertScript()
//    {
//        const string spacer = ", ";
//        var scriptFields = new StringBuilder();
//        var scriptParameters = new StringBuilder();
//        var parameters = new Dictionary<string, object>();
//        _parameters ??= new SortedDictionary<string, object?>();
//        scriptFields.Append($@"INSERT INTO {_tableName}(");
//        scriptParameters.Append(@" VALUES (");

//        foreach (var (key, value) in _parameters)
//        {
//            scriptFields.Append($"[{key}]{spacer}");
//            scriptParameters.Append($"@{key}{spacer}");
//            parameters.Add($"@{key}", StringHelper.GetValue(value));
//        }

//        scriptFields = new StringBuilder(scriptFields.ToString().RemoveLastOccurrence(spacer)).Append(')');
//        scriptParameters = new StringBuilder(scriptParameters.ToString().RemoveLastOccurrence(spacer)).Append(')');

//        if (!string.IsNullOrEmpty(_identityFieldName))
//            scriptParameters.Append(" SELECT SCOPE_IDENTITY();");

//        return ($"{scriptFields}{scriptParameters}", parameters);
//    }

//}