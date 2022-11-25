internal class Program
{
    private static void Main(string[] args)
    {
        var (a, b) = new ClassDeconstruct() { Name = 'F', Age = 36 };
        Console.WriteLine($"{a}: {b}");
    }
}

public class ClassDeconstruct
{
    public char Name { get; set; }
    public int Age { get; set; }
    public void Deconstruct(out char parameter1, out int parameter2)
    {
        parameter1 = Name;
        parameter2 = Age;
    }
}
