using System.Collections.Generic;

namespace DuckGame
{
    public class ModMetaData : BinaryClassChunk
    {
        public bool hasLocalMods;
        public HashSet<ulong> workshopIDs = new HashSet<ulong>();
        public HashSet<string> localModIdentifiers = new HashSet<string>();
    }
}
