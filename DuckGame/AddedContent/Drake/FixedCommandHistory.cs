using System;
using System.IO;
using System.Linq;

namespace DuckGame.AddedContent.Drake;

public static class FixedCommandHistory
{
    public static readonly string CommandHistoryFilePath = DuckFile.optionsDirectory + "CommandHistory.txt";
    
    static FixedCommandHistory()
    {
        MonoMain.OnGameExit += _ => { SaveCommandHistory(); };
    }
    
    [PostInitialize]
    public static void LoadCommandHistory()
    {
        //you don't need to create and load the file if it doesn't exist...
        if (!File.Exists(CommandHistoryFilePath)) return;

        string[] history = File.ReadAllLines(CommandHistoryFilePath);

        DevConsole.core.previousLines.AddRange(history);
        DevConsole.core.lastCommandIndex += history.Length;
    }
    
    public static void SaveCommandHistory()
    {
       try
       {
            File.WriteAllLines(CommandHistoryFilePath, DevConsole.core.previousLines.GetRange(0, Math.Min(25, DevConsole.core.previousLines.Count - 1)));
       }
       catch
       {

       }
    }
}