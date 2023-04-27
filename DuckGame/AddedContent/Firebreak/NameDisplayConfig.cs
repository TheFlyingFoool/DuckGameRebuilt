using Newtonsoft.Json;

namespace DuckGame
{
    [AdvancedConfig("nameDisplay")]
    public class NameDisplayConfig : IAdvancedConfig
    {
        public bool randomfuckingboolean = false;
        [ACMin(0)]
        public float Size;
        public float VerticalSpacing;
        public float HorizontalSpacing;
        [ACMin(0)] [ACMax(1)] [ACIncrementValue(0.05)]
        public float Opacity;
        [ACMin(0)]
        public float TeamLineWidth;
        public DeadPlayerRemoval RemoveDeadPlayers;
        public float XOffset;
        public float YOffset; 
        public ScoreShowing ShowScores;
        
        public void RevertToDefaults()
        {
            Size = 0.5f;
            VerticalSpacing = 2f;
            HorizontalSpacing = 2f;
            Opacity = 1f;
            TeamLineWidth = 1f;
            RemoveDeadPlayers = DeadPlayerRemoval.ShowAsGhosts;
            XOffset = 0;
            YOffset = 0;
            ShowScores = ScoreShowing.DontShow;
        }

        public enum ScoreShowing
        {
            DontShow,
            ShowValue,
            ShowBar
        }

        public enum DeadPlayerRemoval
        {
            NeverRemove,
            RemoveDead,
            ShowAsGhosts
        }
    }
}