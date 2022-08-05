namespace DuckGame;

public static partial class DevConsoleCommands
{
    [DevConsoleCommand]
    public static void Test()
    {
        DevConsole.Log("1");
    }
    
    [DevConsoleCommand(Name = "secondtest")]
    public static void Test2()
    {
        DevConsole.Log("2");
    }
}