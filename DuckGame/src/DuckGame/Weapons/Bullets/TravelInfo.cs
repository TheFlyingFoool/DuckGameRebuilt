// Decompiled with JetBrains decompiler
// Type: DuckGame.TravelInfo
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class TravelInfo
    {
        public Vec2 p1;
        public Vec2 p2;
        public float length;

        public TravelInfo(Vec2 point1, Vec2 point2, float len)
        {
            p1 = point1;
            p2 = point2;
            length = len;
        }
    }
}
