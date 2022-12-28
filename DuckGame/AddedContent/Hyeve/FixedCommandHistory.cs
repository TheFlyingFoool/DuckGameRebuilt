using System;
using System.Collections.Generic;
using DuckGame;

namespace AddedContent.Hyeve
{
    public static class FixedCommandHistory
    {
        [AutoConfigField(External = "CommandHistory")]
        public static List<string> SavedCommandHistory
        {
            get => DevConsole.core.previousLines.FastTakeFromEnd(25);
            set
            {
                foreach (string line in DevConsole.core.previousLines)
                { 
                    if (!DevConsole.core.previousLines.Contains(line)) 
                    {
                        DevConsole.core.previousLines.Add(line);
                        DevConsole.core.lastCommandIndex += 1;
                    }
                }
            }
        }

        private static List<string> FastTakeFromEnd(this IReadOnlyList<string> list, int limit)
        {
            int smartLimit = Math.Min(list.Count, limit);
            List<string> result = new(smartLimit);

            for (int i = list.Count - smartLimit; i < list.Count; i++)
            {
                result.Add(list[i]);
            }

            return result;
        }
    }
}