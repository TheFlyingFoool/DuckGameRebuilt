using System;

namespace DuckGame
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ACMaxAttribute : Attribute
    {
        public readonly double Value;
        
        public ACMaxAttribute(double value)
        {
            Value = value;
        }
    }
}