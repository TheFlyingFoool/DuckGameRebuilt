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
        public int Slider;
        public string String;
        
        public void RevertToDefaults()
        {
            Slider = 5;
        }
    }
}