namespace SQLScriptHelper;

public interface IScriptBuilder<T> where T : class
{
    (string, Dictionary<string, object>) GenerateInsert(T data, string[] fields);
    
}