namespace DuckGame
{
    [AdvancedConfig("experimental")]
    public class ExperimentalConfig : IAdvancedConfig
    {
        public bool Boolean;
        public int SignedInteger;
        public int UnsignedInteger;
        public float Float;
        [ACMin(0)] [ACMax(10)] [ACSlider(2, SecondaryStep = 1)]
        public int Slider = 5;
        public string String;
    }
}