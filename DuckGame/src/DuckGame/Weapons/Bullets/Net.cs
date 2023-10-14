// Decompiled with JetBrains decompiler
// Type: DuckGame.Net
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public class Net : PhysicsObject
    {
        private SpriteMap _sprite;
        protected Duck _owner;

        public Net(float xpos, float ypos, Duck owner)
          : base(xpos, ypos)
        {
            _sprite = new SpriteMap("net", 16, 16);
            graphic = _sprite;
            center = new Vec2(8f, 7f);
            collisionOffset = new Vec2(-6f, -5f);
            collisionSize = new Vec2(12f, 12f);
            depth = -0.5f;
            thickness = 2f;
            weight = 1f;
            _owner = owner;
            _impactThreshold = 0.01f;
        }

        public override void Update()
        {
            if (Math.Abs(hSpeed) + Math.Abs(vSpeed) > 0.1f) 
                angle = -Maths.PointDirectionRad(Vec2.Zero, velocity);
            if (grounded && Math.Abs(vSpeed) + Math.Abs(hSpeed) <= 0f) 
                alpha -= 0.2f;
            if (alpha <= 0f) 
                Level.Remove(this);
            if (!onFire && Level.CheckRect<SmallFire>(position + new Vec2(-4f, -4f), position + new Vec2(4f, 4f), this) != null)
            {
                onFire = true;
                Level.Add(SmallFire.New(0f, 0f, 0f, 0f, stick: this, firedFrom: this));
            }
            base.Update();
        }
        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (Network.isActive && connection != DuckNetwork.localConnection)
            {
                return;
            }
            Duck duck = with as Duck;
            if (duck != null && !duck.inNet && !duck.dead)
            {
                duck.Netted(this);
                if (duck._trapped != null)
                {
                    for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 4; ++index)
                    {
                        SmallSmoke smallSmoke = SmallSmoke.New(duck._trapped.x + Rando.Float(-4f, 4f), duck._trapped.y + Rando.Float(-4f, 4f));
                        smallSmoke.hSpeed += duck._trapped.hSpeed * Rando.Float(0.3f, 0.5f);
                        smallSmoke.vSpeed -= Rando.Float(0.1f, 0.2f);
                        Level.Add(smallSmoke);
                    }
                }
                if (Recorder.currentRecording != null)
                {
                    Recorder.currentRecording.LogBonus();
                    return;
                }
            }
            else
            {
                RagdollPart p = with as RagdollPart;
                if (p != null && p.doll.captureDuck != null && !p.doll.captureDuck.dead)
                {
                    duck = p.doll.captureDuck;
                    Fondle(p.doll);
                    p.doll.Unragdoll();
                    duck.Netted(this);
                    if (duck._trapped != null)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            SmallSmoke smallSmoke2 = SmallSmoke.New(duck._trapped.x + Rando.Float(-4f, 4f), duck._trapped.y + Rando.Float(-4f, 4f));
                            smallSmoke2.hSpeed += duck._trapped.hSpeed * Rando.Float(0.3f, 0.5f);
                            smallSmoke2.vSpeed -= Rando.Float(0.1f, 0.2f);
                            Level.Add(smallSmoke2);
                        }
                    }
                    if (Recorder.currentRecording != null)
                    {
                        Recorder.currentRecording.LogBonus();
                    }
                }
            }
        }
    }
}
