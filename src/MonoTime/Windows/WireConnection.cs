// Decompiled with JetBrains decompiler
// Type: DuckGame.WireConnection
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class WireConnection
    {
        public Vec2 position;
        public WireConnection up;
        public WireConnection down;
        public WireConnection left;
        public WireConnection right;
        public bool wireRight;
        public bool wireLeft;
        public bool wireUp;
        public bool wireDown;
    }
}
