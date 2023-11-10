using System;

namespace DuckGame
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class FixedNetworkID : Attribute
    {
        public ushort FixedID { get; private set; }

        public FixedNetworkID(ushort id) => FixedID = id;
    }
}
