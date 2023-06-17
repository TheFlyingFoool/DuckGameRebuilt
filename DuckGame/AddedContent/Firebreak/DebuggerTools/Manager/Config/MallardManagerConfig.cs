namespace DuckGame.MMConfig
{
    [AdvancedConfig("mallardManager")]
    public sealed class MallardManagerConfig : IAdvancedConfig
    {
        [ACHidden]
        public bool Enabled = true;
        
        [ACHidden]
        public float Zoom = 0f;
        
        [ACMin(0)] [ACMax(1)] [ACSlider(0.1, SecondaryStep = 0.01)]
        public float Opacity = 0.9f;
        
        [ACHeader] [ACColor]
        public MMColorsConfig Colors = new();
        
        [ACHeader]
        public MMConsoleConfig Console = new();
        
        [ACHeader]
        public MMKeymapConfig Keymap = new();
    }
}