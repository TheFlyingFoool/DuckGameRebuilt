using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DuckGame.AddedContent.Drake;

public static class FixedCommandHistory
{
    [AutoConfigField(External = "CommandHistory")]
    public static List<string> SavedCommandHistory
    {
        get => DevConsole.core.previousLines.FastTakeFromEnd(25).Reverse().ToList();
        set
        {
            DevConsole.core.previousLines.AddRange(value);
            DevConsole.core.lastCommandIndex += value.Count;
        }
    }
    
    private static IEnumerable<string> FastTakeFromEnd(this IReadOnlyList<string> list, int limit)
    {
        for (int i = list.Count - 1; i >= 0; i--)
        {
            if (limit --> 0)
                yield return list[i];
            else break;
        }
    }
}