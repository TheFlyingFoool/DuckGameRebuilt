using AddedContent.Firebreak;
using System;
using System.Collections.Generic;

namespace DuckGame.ConsoleEngine
{
    public static partial class Commands
    {
        [return: PrintSerialized]
        [Marker.DevConsoleCommand(Description = "Creates a collection of incrementing integers", To = ImplementTo.DuckShell)]
        public static int[] Range(int size, int start = 0, int step = 1)
        {
            if (size == 0)
                return Array.Empty<int>();

            if (size < 0)
                throw new Exception("Invalid range size");

            int[] range = new int[size];

            for (int i = start, j = 0; j < size; i += step, j++)
            {
                range[j] = i;
            }

            return range;
        }
    }
}