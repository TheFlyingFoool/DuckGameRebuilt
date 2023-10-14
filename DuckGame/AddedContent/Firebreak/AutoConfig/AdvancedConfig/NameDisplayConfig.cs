using AddedContent.Firebreak;
using Newtonsoft.Json;

namespace DuckGame
{
    [Marker.AdvancedConfig("nameDisplay")]
    public class NameDisplayConfig
    {
        public float FontSize = 0.5f;
        public float VerticalSpacing = 2f;
        public float HorizontalSpacing = 2f;
        public float Opacity = 1f;
        public float TeamLineWidth = 1f;
        public DeadPlayerRemoval RemoveDeadPlayers = DeadPlayerRemoval.Ghost;
        public float XOffset = 0;
        public float YOffset = 0;
        public ScoreShowing ShowScores = ScoreShowing.Value;

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