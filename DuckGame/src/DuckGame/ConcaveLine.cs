using System.Collections.Generic;

namespace DuckGame
{
    public class ConcaveLine
    {
        public Vec2 p1;
        public Vec2 p2;
        public int index;
        public List<ConcaveLine> intersects = new List<ConcaveLine>();

        public ConcaveLine(Vec2 p1val, Vec2 p2val)
        {
            p1 = p1val;
            p2 = p2val;
        }
    }
}
