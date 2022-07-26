// Decompiled with JetBrains decompiler
// Type: DuckGame.ChallengeModeNew
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
            this._contextMenuFilter.Add("random");
            this._contextMenuFilter.Add("music");
            this._editorName = "Challenge Settings";
            this.editorTooltip = "Place this in your level to make it a challenge, and set trophies!";
            this.Music._tooltip = "The music to play during this challenge, should be the name of a song in the Duck Game folder.";
            this.TargetReticles._tooltip = "If enabled, off screen targets will show a reticle at the edge of the screen.";
        }

        public override void Initialize()
        {
            this.music.value = this.Music.value;
            base.Initialize();
            ChallengeMode.showReticles = this.TargetReticles.value;
        }

        public override void PrepareCounts()
        {
            base.PrepareCounts();
            this._challenge.countGoodies = Level.current.things[typeof(Goody)].Count<Thing>() > 0;
            this._challenge.countTargets = Level.current.things[typeof(TargetDuckNew)].Count<Thing>() > 0 || Level.current.things[typeof(GoalType)].Count<Thing>() > 0;
        }
    }
}
