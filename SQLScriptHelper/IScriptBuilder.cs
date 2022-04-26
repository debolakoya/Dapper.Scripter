namespace SQLScriptHelper;

public interface IScriptBuilder<T> where T : class
{
    (string, Dictionary<string, object>) GenerateInsert(T data, string[] fields);
    (string, Dictionary<string, object>) GenerateUpdate(T data, string[] fields, (Dictionary<string,object> parameters, string clause) whereClause);
}