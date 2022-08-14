using System;
using System.Collections.Generic;
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
        IEnumerable<string> lines = DevConsole.core.previousLines.FastTake(25);
        
        File.WriteAllLines(CommandHistoryFilePath, lines);
    }

    private static IEnumerable<string> FastTake(this IReadOnlyList<string> list, int limit)
    {
        for (int i = 0; i < limit && i < list.Count; i++)
        {
            yield return list[i];
        }
    }
}