// Decompiled with JetBrains decompiler
// Type: DuckGame.BlockCorner
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    public class BlockCorner
    {
        public Vec2 corner;
        public Block block;
        public bool wallCorner;
        public List<BlockCorner> testedCorners = new List<BlockCorner>();

        public BlockCorner(Vec2 c, Block b, bool wall = false)
        {
            corner = c;
            block = b;
            wallCorner = wall;
        }

        public BlockCorner Copy() => new BlockCorner(corner, block, wallCorner);
    }
}
