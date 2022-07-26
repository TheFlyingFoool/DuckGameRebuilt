// Decompiled with JetBrains decompiler
// Type: DuckGame.SequenceCrate
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Special", EditorItemType.Arcade)]
    [BaggedProperty("isInDemo", true)]
    public class SequenceCrate : Crate, ISequenceItem
    {
        public SequenceCrate(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this.sequence = new SequenceItem((Thing)this);
            this.sequence.type = SequenceItemType.Goody;
            this._editorName = "Seq Crate";
        }

        protected override bool OnDestroy(DestroyType type = null)
        {
            if (this.sequence != null && this.sequence.isValid)
            {
                this.sequence.Finished();
                if (ChallengeLevel.running)
                    ++ChallengeLevel.goodiesGot;
            }
            return base.OnDestroy(type);
        }
    }
}
