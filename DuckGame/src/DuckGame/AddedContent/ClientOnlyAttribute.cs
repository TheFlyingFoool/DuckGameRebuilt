using System;

namespace DuckGame
{
   
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ClientOnlyAttribute : Attribute
    {
        public ClientOnlyAttribute()
        {
        }
        public ClientOnlyAttribute(ushort id)
        {
        }
    }
}
