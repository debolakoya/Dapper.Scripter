namespace ConsoleApp1;

public class User
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int Number { get; set; }
    public decimal Amount { get; set; }
    public DateOnly? DateofBirth { get; set; }
    public TimeOnly TimeOfDay { get; set; }
    public DateTime ActionDateTime { get; set; }
    public Guid? GuidField { get; set; }
    public double AmountDouble { get; set; }
}

public class School
{
    public int Id { get; set; }
    public string? Name { get; set; }
}