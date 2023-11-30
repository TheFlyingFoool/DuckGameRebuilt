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
