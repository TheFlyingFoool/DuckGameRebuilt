using System;
using System.Web.UI.WebControls.WebParts;

namespace DuckGame
{
    public struct IntRange
    {
        public int Start;
        public int End;

        public int Length => End - Start;
        public int LengthAbs => Math.Abs(End - Start);
        
        public IntRange(int start, int end)
        {
            Start = start;
            End = end;
        }
    }
}