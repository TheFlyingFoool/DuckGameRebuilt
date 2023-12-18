using System.Collections.Generic;

namespace DuckGame
{
    public class DamageHit
    {
        public Thing thing;
        public List<Vec2> points = new List<Vec2>();
        public List<DamageType> types = new List<DamageType>();
    }
}
