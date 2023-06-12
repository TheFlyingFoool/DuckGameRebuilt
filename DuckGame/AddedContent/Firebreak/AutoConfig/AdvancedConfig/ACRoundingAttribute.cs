using System;

namespace DuckGame
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ACRoundingAttribute : Attribute
    {
        public readonly int Decimals;

        public ACRoundingAttribute(int decimals)
        {
            Decimals = decimals;
        }
    }
}