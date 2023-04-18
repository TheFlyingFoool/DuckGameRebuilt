using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class DynamicDCLine
    {
        public int LineIndex;
        public readonly Func<int, string?> GetLine;
        private int _iteration;
        
        public DynamicDCLine(Func<int, string?> lineFunction)
        {
            GetLine = lineFunction;
        }

        public void Done()
        {
            s_allDynamicLines.Remove(this);
        }

        public void Log()
        {
            string line = GetLine(_iteration--);
            
            if (line != null)
            {
                LineIndex = DevConsole.core.lines.Count;
                DevConsole.Log(line);
                s_allDynamicLines.Add(this);
                return;
            }
            
            Done();
        }

        private static List<DynamicDCLine> s_allDynamicLines = new();

        [DrawingContext(CustomID = "dyn_dc_update")]
        public static void UpdateLines()
        {
            for (int i = 0; i < s_allDynamicLines.Count; i++)
            {
                DynamicDCLine item = s_allDynamicLines[i];

                string line = item.GetLine(item._iteration++);

                if (line != null && item._iteration > 0)
                    DevConsole.core.lines.ElementAt(item.LineIndex).line = line;
                else if (item._iteration != 0)
                    item.Done();
            }
        }
    }
}