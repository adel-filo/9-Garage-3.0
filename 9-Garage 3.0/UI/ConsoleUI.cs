public class ConsoleUI : IUI
{
    public void WriteLine(string message = "")
    {
        Console.WriteLine(message);
    }

    public void Write(string message)
    {
        Console.Write(message);
    }

    public string ReadLine()
    {
        return Console.ReadLine() ?? string.Empty;
    }

    public void Clear()
    {
        Console.Clear();
    }
}