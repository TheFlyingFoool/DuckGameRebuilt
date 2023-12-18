using System.Collections.Generic;

namespace DuckGame
{
    public class BlockStructure
    {
        public List<BlockCorner> corners = new List<BlockCorner>();
        public HashSet<Block> blocks = new HashSet<Block>();
    }
}
