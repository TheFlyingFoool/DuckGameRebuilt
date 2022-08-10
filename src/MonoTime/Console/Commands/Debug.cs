using System.Linq;

namespace DuckGame;

public static partial class DevConsoleCommands
{
    [DevConsoleCommand]
    public static void Debug(int i)
    {
        switch (i)
        {
            case 0:
            {
                DevConsole.ConsoleLineOffset = 0;
                break;
            }
            case 1:
            {
                for (int j = 0; j < 120; j++)
                {
                    DevConsole.Log(new string(new char[Rando.Int(10, 73)].Select(_ => (char) Rando.Int(32, 126)).ToArray()));
                }
                break;
            }
        }
    }
}