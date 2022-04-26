// See https://aka.ms/new-console-template for more information

using ConsoleApp1;
using ScriptBuilder;
using ScriptBuilder.Builders;
var school = new School
{
    Id = 1,
    Name="School 1"
};

var user = new User
{
    Amount = 35690,
    AmountDouble = 356.90,
    Id = 6,
    Name = "Bola",
    Number = 90,
    ActionDateTime = new DateTime(1990, 1, 1, 12, 10, 30, DateTimeKind.Utc),
}; 
var (insertScript, insertParam) = InsertBuilder.OfType<User>( "tbl_Users")
    .AddField("Id")
    .AddFields()
    .Except(nameof(user.Id))
    .Build(user);

Console.WriteLine(insertScript);
foreach (var val in insertParam)
{
    Console.WriteLine($"{val.Key}:{val.Value}");
}

Console.WriteLine("===========================");
var (updateScript, updateParam) = UpdateBuilder.OfType<User>(nameof(User))
    .AddFields()
    .Except(nameof(user.Id))
    //.Where(nameof(user.Id), Clause.Equals, user.Id)
    //.WhereAnd(nameof(user.Name), Clause.Equals, user.Name)
    //.WhereBetween(nameof(user.Id), 77, 100)
    //.WhereOr(nameof(user.Id), Clause.Equals, user.Name)
    .Build(user);


Console.WriteLine(updateScript);
foreach (var val in updateParam)
{
    Console.WriteLine($"{val.Key}:{val.Value}");
}
