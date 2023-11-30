using NAudio.MediaFoundation;
using System;
using System.Collections.Generic;

namespace DuckGame
{
    [EditorGroup("Special", EditorItemType.Arcade)]
    [BaggedProperty("isInDemo", true)]
    public class SequenceCrate : Crate, ISequenceItem
    {
        public int _variant;
        public static DuckPersona[] _variantPersonas =
        {
            Persona.Duck7,
            Persona.Duck6,
            Persona.Duck2,
            Persona.Duck3
        };

        public SequenceCrate(float xpos, float ypos)
          : base(xpos, ypos)
        {
            sequence = new SequenceItem(this)
            {
                type = SequenceItemType.Goody
            };
            _editorName = "Seq Crate";
        }
        public bool appliedSeq;
        public override void Update()
        {
            if (DGRSettings.SequenceCrateRetexture && !appliedSeq)
            {
                appliedSeq = true;
                int var = Math.Abs((int)(x / 11f) * 11 + (int)(y / 11f) * 11);
                _variant = var % 4;
                _spriteAccessor = new SpriteMap($"seq_crate_{_variant}", 16, 16);
                graphic = _spriteAccessor;
                _onHitSFX = "swipe";
                _onCollideSFX = "presentLand";
                _onDestroySFX = "rainpop"; // "page", "little_punch", "lightMatch"
            }
            base.Update();
        }
        public override void ExitHit(Bullet bullet, Vec2 exitPos)
        {
            if (DGRSettings.SequenceCrateRetexture)
            {
                for (int index = 0; index < DGRSettings.ActualParticleMultiplier * (1f + damageMultiplier / 2f); ++index)
                {
                    Feather woodDebris = Feather.New(exitPos.x, exitPos.y, _variantPersonas[_variant]);
                    woodDebris.hSpeed = (bullet.travelDirNormalized.x * 3f * (Rando.Float(1f) + 0.3f));
                    woodDebris.vSpeed = (bullet.travelDirNormalized.y * 3f * (Rando.Float(1f) + 0.3f) - (Rando.Float(2f) - 1f));
                    Level.Add(woodDebris);
                }
            }
            else base.ExitHit(bullet, exitPos);
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
