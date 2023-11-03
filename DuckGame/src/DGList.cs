using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class DGList
    {
        public static void Sort<T>(List<T> list, Comparison<T> comparison) => list.Sort(comparison);
    }
}
