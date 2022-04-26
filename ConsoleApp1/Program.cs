// See https://aka.ms/new-console-template for more information

using ConsoleApp1;
using SQLScriptHelper;

var scriptHelper = new ScriptBuilder<User>("Id");
var user = new User
{
  Amount = 35690,
  AmountDouble = 356.90,
  Id = 6,
  Name = "Bola",
  Number = 90,
  DateofBirth = new DateTime(2022, 8, 4)
};

// var fields1=   FieldBuilder.OfType<User>()
//        .AddField(nameof(user.Amount))
//        .Except(nameof(user.Id));

var fields2 = FieldBuilder.OfType<User>()
    .AddFields()
    .Except(nameof(user.Id));



var (script, param) = scriptHelper.GenerateInsert(user, fields2.ToArray());
Console.WriteLine(script);
foreach (var val in param)
{
  Console.WriteLine($"{val.Key}:{val.Value}");
}

Console.WriteLine("===========================");

var updateFields = FieldBuilder.OfType<User>()
    .AddFields()
    .Except(nameof(user.Id));

var keys = KeyBuilder.OfType<User>()
    .Where(nameof(user.Id),KeyBuilderClause.Equals,user.Id)
    .WhereAnd(nameof(user.Name), KeyBuilderClause.Equals, user.Name)
    .WhereBetween(nameof(user.Id),77,100)
    .WhereOr(nameof(user.Id),KeyBuilderClause.Equals, user.Name)
    .Build() ;

var (script1, param1) = scriptHelper.GenerateUpdate(user, updateFields.ToArray(),keys);
Console.WriteLine(script1);
foreach (var val in param1)
{
  Console.WriteLine($"{val.Key}:{val.Value}");
}