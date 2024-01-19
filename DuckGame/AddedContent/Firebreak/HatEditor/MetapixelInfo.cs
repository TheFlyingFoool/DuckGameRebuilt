using System;

namespace DuckGame
{
    public struct MetapixelInfo
    {
        public byte Index;
        public string Name;
        public string Description;
        public bool DGRExclusive;
        public bool VanillaSynced;
        public Type MDType;

        public MetapixelInfo(byte index, string name, string description, Type type, bool dgrExclusive, bool vanillaSynced)
        {
            Index = index;
            Name = name;
            Description = description;
            MDType = type;
            DGRExclusive = dgrExclusive;
            VanillaSynced = vanillaSynced;
        }
    }
}