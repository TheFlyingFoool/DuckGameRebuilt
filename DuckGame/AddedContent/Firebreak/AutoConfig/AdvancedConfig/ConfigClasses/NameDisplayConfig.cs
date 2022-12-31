using Newtonsoft.Json;

namespace DuckGame
{
    [AdvancedConfig("nameDisplay")]
    public class NameDisplayConfig : IAdvancedConfig
    {
        public float FontSize;
        public float VerticalSpacing;
        public float HorizontalSpacing;
        public float Opacity;
        public float TeamLineWidth;
        public bool RemoveDeadPlayers;
        public float XOffset;
        public float YOffset; 
        
        public void RevertToDefaults()
        {
            FontSize = 0.5f;
            VerticalSpacing = 2f;
            HorizontalSpacing = 2f;
            Opacity = 1f;
            TeamLineWidth = 1f;
            RemoveDeadPlayers = true;
            XOffset = 0;
            YOffset = 0;
        }
    }
}