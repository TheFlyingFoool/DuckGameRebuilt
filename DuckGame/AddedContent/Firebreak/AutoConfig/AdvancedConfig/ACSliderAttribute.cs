using System;

namespace DuckGame
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ACSliderAttribute : Attribute
    {
        public readonly double Step;
        public double SecondaryStep { get; set; }
        
        public ACSliderAttribute(double step)
        {
            Step = step;
        }
    }
}