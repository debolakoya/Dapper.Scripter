// See https://aka.ms/new-console-template for more information

using ConsoleApp1;
using ScriptBuilder.Builders;

var school = new School
{
  Id = 1,
  Name = ""
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
GenerateInsert(user);
GenerateUpdate(user);



void GenerateInsert(User user1)
{
  var (insertScript, insertParam) = Builder.OfType<User>("tbl_Users", "Id")
      .AddField("Name")
      .Except(nameof(user1.Id))
      .Insert(user1);

  Console.WriteLine(insertScript);
  foreach (var val in insertParam)
  {
    Console.WriteLine($"{val.Key}:{val.Value}");
  }
}

void GenerateUpdate(User user)
{
  var (updateScript, updateParam) = Builder.OfType<User>("mq", "Id")
    .AddField("Name")
     .Except(nameof(user.DateofBirth))
    //.Where(nameof(user.Id), Clause.Equals, user.Id)
    //.WhereAnd(nameof(user.Name), Clause.Equals, user.Name)
    //.WhereBetween(nameof(user.Id), 77, 100)
    //.WhereOr(nameof(user.Id), Clause.Equals, user.Name)
    .Update(user);


  Console.WriteLine(updateScript);
  foreach (var val in updateParam)
  {
    Console.WriteLine($"{val.Key}:{val.Value}");
  }
}
