using System.Linq;

namespace DuckGame
{
    [EditorGroup("Arcade", EditorItemType.ArcadeNew)]
    [BaggedProperty("isOnlineCapable", false)]
    [BaggedProperty("previewPriority", true)]
    public class ChallengeModeNew : ChallengeMode
    {
        public EditorProperty<string> Music = new EditorProperty<string>("");
        public EditorProperty<bool> TargetReticles = new EditorProperty<bool>(true);

        public ChallengeModeNew()
        {
            _contextMenuFilter.Add("random");
            _contextMenuFilter.Add("music");
            _editorName = "Challenge Settings";
            editorTooltip = "Place this in your level to make it a challenge, and set trophies!";
            Music._tooltip = "The music to play during this challenge, should be the name of a song in the Duck Game folder.";
            TargetReticles._tooltip = "If enabled, off screen targets will show a reticle at the edge of the screen.";
        }

        public override void Initialize()
        {
            music.value = Music.value;
            base.Initialize();
            showReticles = TargetReticles.value;
        }

        public override void PrepareCounts()
        {
            base.PrepareCounts();
            _challenge.countGoodies = Level.current.things[typeof(Goody)].Count() > 0;
            _challenge.countTargets = Level.current.things[typeof(TargetDuckNew)].Count() > 0 || Level.current.things[typeof(GoalType)].Count() > 0;
        }
    }
}
