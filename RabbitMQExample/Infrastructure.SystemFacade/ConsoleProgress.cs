namespace SystemFacade;

public class ConsoleProgress: IProgress<string>
{
    public void Report(string message)
    {
        var infoText = $"{DateTime.Now}: ";
        var colorBackup = Console.ForegroundColor;
        WriteColorLine(infoText, ConsoleColor.DarkYellow);
        WriteColorLine(message, colorBackup);
        Console.WriteLine();
    }
    
    private static void WriteColorLine(string message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.Write(message);
    }
}