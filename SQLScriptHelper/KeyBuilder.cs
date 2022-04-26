// See https://aka.ms/new-console-template for more information
using System.Text;

namespace SQLScriptHelper;

public static class KeyBuilder
{
  private static Type? _type;

  public static (Dictionary<string,object> keyValues, StringBuilder clause) OfType<T>() where T : class
  {
    _type = typeof(T);
    return new(new Dictionary<string,object>(), new StringBuilder);
  }


  public static (Dictionary<string,object> keyValues, StringBuilder clause) Where(this (Dictionary<string,object> keyValues, StringBuilder clause) keybuilderResult, string field, KeyBuilderClause clause, object value)
  {
    if (_type == null)
      throw new InvalidOperationException($"FieldBuilder is not initialized. Please call OfType method first.");

    var property = _type.GetProperty(field);

    if (property == null)
      throw new KeyNotFoundException($"{field} does not exist in {nameof(_type)}");

    if (!keybuilderResult.keyValues.Keys.Contains(field, StringComparer.OrdinalIgnoreCase))
    {
      keybuilderResult.keyValues.Add(field,value);
    }
    
    return keybuilderResult;
  }
  
  public static (Dictionary<string,object> keyValues, StringBuilder clause) WhereAnd(this (Dictionary<string,object> keyValues, StringBuilder clause) keybuilderResult, string field, KeyBuilderClause clause, object value)
  {
    throw new NotImplementedException();
  }
  public static (Dictionary<string,object> keyValues, StringBuilder clause) WhereBetween(this (Dictionary<string,object> keyValues, StringBuilder clause) keybuilderResult, string field, object value1, object value2)
  {
    throw new NotImplementedException();
  }
  public static (Dictionary<string,object> keyValues, StringBuilder clause) WhereOr(this (Dictionary<string,object> keyValues, StringBuilder clause) keybuilderResult, string field, KeyBuilderClause clause, object value)
  {
    throw new NotImplementedException();
  }

  public static (Dictionary<string,object> keyValues, StringBuilder clause) WhereIn(this (Dictionary<string,object> keyValues, StringBuilder clause) keybuilderResult, string field, IEnumerable<object> values)
  {
    throw new NotImplementedException();
  }

  public static (Dictionary<string,object> keyValues, StringBuilder clause) WhereNotIn(this (Dictionary<string,object> keyValues, StringBuilder clause) keybuilderResult, string field, IEnumerable<object> values)
  {
    throw new NotImplementedException();
  }

  public static (Dictionary<string,object> keyValues, StringBuilder clause) WhereLike(this (Dictionary<string,object> keyValues, StringBuilder clause) keybuilderResult, string field, object value)
  {
    throw new NotImplementedException();
  }
  
  public static (Dictionary<string,object> keyValues, StringBuilder clause) WhereNotLike(this (Dictionary<string,object> keyValues, StringBuilder clause) keybuilderResult, string field, object value)
  {
    throw new NotImplementedException();
  }

  public static (Dictionary<string,object> keyValues, StringBuilder clause) WhereNull(this (Dictionary<string,object> keyValues, StringBuilder clause) keybuilderResult, string field)
  {
    throw new NotImplementedException();
  }

  public static (Dictionary<string,object> keyValues, StringBuilder clause) WhereNotNull(this (Dictionary<string,object> keyValues, StringBuilder clause) keybuilderResult, string field)
  {
    throw new NotImplementedException();
  }

  public static (Dictionary<string,object> keyValues, string clause) Build(this (Dictionary<string,object> keyValues, StringBuilder clause) keybuilderResult)
  {
    throw new NotImplementedException();
  }
}
