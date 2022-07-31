// Decompiled with JetBrains decompiler
// Type: DuckGame.NMPop
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class NMPop : NMEvent
    {
        public Vec2 position;

        public NMPop()
        {
        }

        public NMPop(Vec2 pPosition) => this.position = pPosition;

        public override void Activate()
        {
            NMPop.AmazingDisappearingParticles(this.position);
            base.Activate();
        }

        public static void AmazingDisappearingParticles(Vec2 pPos)
        {
            Level.Add(new PopEffect(pPos.x, pPos.y));
            for (int index = 0; index < 6; ++index)
            {
                Level.Add(ConfettiParticle.New(pPos.x + Rando.Float(-6f, 0f), pPos.y + Rando.Float(-6f, 6f), new Vec2(Rando.Float(-1f, 0f), Rando.Float(-1f, 1f))));
                Level.Add(ConfettiParticle.New(pPos.x + Rando.Float(0f, 6f), pPos.y + Rando.Float(-6f, 6f), new Vec2(Rando.Float(0f, 1f), Rando.Float(-1f, 1f))));
                Level.Add(ConfettiParticle.New(pPos.x + Rando.Float(-6f, 0f), pPos.y + Rando.Float(-6f, 6f), new Vec2(Rando.Float(-1f, 0f), Rando.Float(-1f, 1f)), lineType: true));
                Level.Add(ConfettiParticle.New(pPos.x + Rando.Float(0f, 6f), pPos.y + Rando.Float(-6f, 6f), new Vec2(Rando.Float(0f, 1f), Rando.Float(-1f, 1f)), lineType: true));
            }
            SFX.Play("balloonPop", pitch: Rando.Float(-0.15f, 0.15f));
        }

        public static void AmazingDisappearingParticles(Vec2 pPos, Layer pLayer)
        {
            PopEffect popEffect = new PopEffect(pPos.x, pPos.y)
            {
                layer = pLayer
            };
            Level.Add(popEffect);
            for (int index = 0; index < 6; ++index)
            {
                ConfettiParticle confettiParticle1 = ConfettiParticle.New(pPos.x + Rando.Float(-6f, 0f), pPos.y + Rando.Float(-6f, 6f), new Vec2(Rando.Float(-1f, 0f), Rando.Float(-1f, 1f)));
                confettiParticle1.layer = pLayer;
                Level.Add(confettiParticle1);
                ConfettiParticle confettiParticle2 = ConfettiParticle.New(pPos.x + Rando.Float(0f, 6f), pPos.y + Rando.Float(-6f, 6f), new Vec2(Rando.Float(0f, 1f), Rando.Float(-1f, 1f)));
                confettiParticle2.layer = pLayer;
                Level.Add(confettiParticle2);
                ConfettiParticle confettiParticle3 = ConfettiParticle.New(pPos.x + Rando.Float(-6f, 0f), pPos.y + Rando.Float(-6f, 6f), new Vec2(Rando.Float(-1f, 0f), Rando.Float(-1f, 1f)), lineType: true);
                confettiParticle3.layer = pLayer;
                Level.Add(confettiParticle3);
                ConfettiParticle confettiParticle4 = ConfettiParticle.New(pPos.x + Rando.Float(0f, 6f), pPos.y + Rando.Float(-6f, 6f), new Vec2(Rando.Float(0f, 1f), Rando.Float(-1f, 1f)), lineType: true);
                confettiParticle4.layer = pLayer;
                Level.Add(confettiParticle4);
            }
            SFX.Play("balloonPop", pitch: Rando.Float(-0.15f, 0.15f));
        }
    }
}
