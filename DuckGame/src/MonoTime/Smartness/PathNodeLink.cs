// Decompiled with JetBrains decompiler
// Type: DuckGame.PathNodeLink
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class PathNodeLink
    {
        public Thing link;
        public Thing owner;
        public float distance;
        public bool oneWay;
        public bool gap;

        public Vec2 position => owner.position;
    }
}
