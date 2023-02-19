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
        public DeadPlayerRemoval RemoveDeadPlayers;
        public float XOffset;
        public float YOffset; 
        public ScoreShowing ShowScores;
        
        public void RevertToDefaults()
        {
            FontSize = 0.5f;
            VerticalSpacing = 2f;
            HorizontalSpacing = 2f;
            Opacity = 1f;
            TeamLineWidth = 1f;
            RemoveDeadPlayers = DeadPlayerRemoval.Ghost;
            XOffset = 0;
            YOffset = 0;
            ShowScores = ScoreShowing.False;
        }

        public enum ScoreShowing
        {
            False,
            Value,
            Bar
        }

        public enum DeadPlayerRemoval
        {
            False,
            True,
            Ghost
        }
    }
}