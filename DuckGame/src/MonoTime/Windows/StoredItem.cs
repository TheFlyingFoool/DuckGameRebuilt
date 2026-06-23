using System;
namespace DuckGame
{
    public class StoredItem
    {
        public BinaryClassChunk serializedData;
        public Thing thing;
        public Type type; // added for DuckUtils
        public Sprite sprite; // added for DuckUtils
    }
}
