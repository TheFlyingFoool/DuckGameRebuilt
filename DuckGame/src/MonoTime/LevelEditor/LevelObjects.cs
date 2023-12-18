using System.Collections.Generic;

namespace DuckGame
{
    public class LevelObjects : BinaryClassChunk
    {
        public List<BinaryClassChunk> objects = new List<BinaryClassChunk>();

        public void Add(BinaryClassChunk obj) => objects.Add(obj);
    }
}
