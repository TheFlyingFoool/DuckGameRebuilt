using System.Collections.Generic;

namespace DuckGame
{
    [EditorGroup("Special", EditorItemType.Arcade)]
    [BaggedProperty("isInDemo", true)]
    public class SequenceCrate : Crate, ISequenceItem
    {
        public int _variant = Rando.Int(1, 4);
        public static Dictionary<int, DuckPersona> _variantPersonaMap = new()
        {
            {1, Persona.Duck7},
            {2, Persona.Duck6},
            {3, Persona.Duck2},
            {4, Persona.Duck3},
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
