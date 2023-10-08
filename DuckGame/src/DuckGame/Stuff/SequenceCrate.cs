// Decompiled with JetBrains decompiler
// Type: DuckGame.SequenceCrate
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    [EditorGroup("Special", EditorItemType.Arcade)]
    [BaggedProperty("isInDemo", true)]
    public class SequenceCrate : Crate, ISequenceItem
    {
        public int _variant = Rando.Int(1, 3);
        public static Dictionary<int, DuckPersona> _variantPersonaMap = new()
        {
            {1, Persona.Duck7},
            {2, Persona.Duck6},
            {3, Persona.Duck2},
        };
        
        public SequenceCrate(float xpos, float ypos)
          : base(xpos, ypos)
        {
            if (DGRSettings.SequenceCrateRetexture)
            {
                _spriteAccessor = new SpriteMap($"seq_crate_{_variant}", 16, 16);
                graphic = _spriteAccessor;
                _onHitSFX = "swipe";
                _onCollideSFX = "presentLand";
                _onDestroySFX = "rainpop"; // "page", "little_punch", "lightMatch"
            }

            sequence = new SequenceItem(this)
            {
                type = SequenceItemType.Goody
            };
            _editorName = "Seq Crate";
        }

        protected override bool OnDestroy(DestroyType type = null)
        {
            if (sequence != null && sequence.isValid)
            {
                sequence.Finished();
                if (ChallengeLevel.running)
                    ++ChallengeLevel.goodiesGot;
            }
            return base.OnDestroy(type);
        }
    }
}
