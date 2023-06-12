using Newtonsoft.Json;
using System.Drawing;

namespace DuckGame
{
    [AdvancedConfig("nameDisplay")]
    public class NameDisplayConfig : IAdvancedConfig
    {
        [ACMin(0)]
        public float Size;
        public float VerticalSpacing;
        public float HorizontalSpacing;
        [ACMin(0)] [ACMax(1)] [ACSlider(0.05)] [ACDiscrete]
        public float Opacity;
        [ACMin(0)]
        public float TeamLineWidth;
        public DeadPlayerRemoval RemoveDeadPlayers;
        [ACScreenPosition]
        public PointF PositionOffset;
        public ScoreShowing ShowScores;
        
        public void RevertToDefaults()
        {
            Size = 0.5f;
            VerticalSpacing = 2f;
            HorizontalSpacing = 2f;
            Opacity = 1f;
            TeamLineWidth = 1f;
            RemoveDeadPlayers = DeadPlayerRemoval.ShowAsGhosts;
            PositionOffset = new PointF(0, 0);
            ShowScores = ScoreShowing.ShowValue;
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