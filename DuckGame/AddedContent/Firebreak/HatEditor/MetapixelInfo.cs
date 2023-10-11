using System;

namespace DuckGame
{
    public struct MetapixelInfo
    {
        public byte Index;
        public string Name;
        public string Description;
        public Type MDType;

        public MetapixelInfo(byte index, string name, string description, Type type)
        {
            Index = index;
            Name = name;
            Description = description;
            MDType = type;
        }
    }
}