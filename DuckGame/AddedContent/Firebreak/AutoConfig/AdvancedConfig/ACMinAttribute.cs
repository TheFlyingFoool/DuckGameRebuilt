using System;

namespace DuckGame
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ACMinAttribute : Attribute
    {
        public readonly double Value;
        
        public ACMinAttribute(double value)
        {
            Value = value;
        }
    }
}