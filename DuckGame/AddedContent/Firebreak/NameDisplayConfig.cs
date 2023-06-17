using Newtonsoft.Json;
using System.Drawing;

namespace DuckGame
{
    [AdvancedConfig("nameDisplay")]
    public class NameDisplayConfig : IAdvancedConfig
    {
        [ACMin(0)]
        public float Size = 0.5f;
        public float VerticalSpacing = 2f;
        public float HorizontalSpacing = 2f;
        [ACMin(0)] [ACMax(1)] [ACSlider(0.05)] [ACDiscrete]
        public float Opacity = 1f;
        [ACMin(0)]
        public float TeamLineWidth = 1f;
        public DeadPlayerRemoval RemoveDeadPlayers = DeadPlayerRemoval.ShowAsGhosts;
        [ACScreenPosition]
        public PointF PositionOffset = PointF.Empty;
        public ScoreShowing ShowScores = ScoreShowing.ShowValue;

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