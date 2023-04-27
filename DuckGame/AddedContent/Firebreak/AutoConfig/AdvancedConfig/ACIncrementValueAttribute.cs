using System;

namespace DuckGame
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ACIncrementValueAttribute : Attribute
    {
        public readonly double Value;
        
        public ACIncrementValueAttribute(double value)
        {
            Value = value;
        }
    }
}