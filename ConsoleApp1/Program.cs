// See https://aka.ms/new-console-template for more information

using ConsoleApp1;
using SQLScriptHelper;

var scriptHelper = new  ScriptBuilder<User>("Id");
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
    
var fields2=   FieldBuilder.OfType<User>()
    .AddFields()
    .Except(nameof(user.Id));
 


var (script, param) = scriptHelper.GenerateInsert(user, fields2.ToArray());
Console.WriteLine(script);
foreach (var val in param)
{
    Console.WriteLine($"{val.Key}:{val.Value}");
}
