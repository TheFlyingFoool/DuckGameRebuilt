// Decompiled with JetBrains decompiler
// Type: DuckGame.Net
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            this._sprite = new SpriteMap("net", 16, 16);
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(8f, 7f);
            this.collisionOffset = new Vec2(-6f, -5f);
            this.collisionSize = new Vec2(12f, 12f);
            this.depth = - 0.5f;
            this.thickness = 2f;
            this.weight = 1f;
            this._owner = owner;
            this._impactThreshold = 0.01f;
        }

        public override void Update()
        {
            if ((double)Math.Abs(this.hSpeed) + (double)Math.Abs(this.vSpeed) > 0.100000001490116)
                this.angleDegrees = -Maths.PointDirection(Vec2.Zero, new Vec2(this.hSpeed, this.vSpeed));
            if (this.grounded && (double)Math.Abs(this.vSpeed) + (double)Math.Abs(this.hSpeed) <= 0.0)
                this.alpha -= 0.2f;
            if ((double)this.alpha <= 0.0)
                Level.Remove((Thing)this);
            if (!this.onFire && Level.CheckRect<SmallFire>(this.position + new Vec2(-4f, -4f), this.position + new Vec2(4f, 4f), (Thing)this) != null)
            {
                this.onFire = true;
                Level.Add((Thing)SmallFire.New(0.0f, 0.0f, 0.0f, 0.0f, stick: ((MaterialThing)this), firedFrom: ((Thing)this)));
            }
            base.Update();
        }

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (Network.isActive && this.connection != DuckNetwork.localConnection)
                return;
            switch (with)
            {
                case Duck duck when !duck.inNet && !duck.dead:
                    duck.Netted(this);
                    if (duck._trapped != null)
                    {
                        for (int index = 0; index < 4; ++index)
                        {
                            SmallSmoke smallSmoke = SmallSmoke.New(duck._trapped.x + Rando.Float(-4f, 4f), duck._trapped.y + Rando.Float(-4f, 4f));
                            smallSmoke.hSpeed += duck._trapped.hSpeed * Rando.Float(0.3f, 0.5f);
                            smallSmoke.vSpeed -= Rando.Float(0.1f, 0.2f);
                            Level.Add((Thing)smallSmoke);
                        }
                    }
                    if (Recorder.currentRecording == null)
                        break;
                    Recorder.currentRecording.LogBonus();
                    break;
                case RagdollPart ragdollPart when ragdollPart.doll.captureDuck != null && !ragdollPart.doll.captureDuck.dead:
                    Duck captureDuck = ragdollPart.doll.captureDuck;
                    this.Fondle((Thing)ragdollPart.doll);
                    ragdollPart.doll.Unragdoll();
                    captureDuck.Netted(this);
                    if (captureDuck._trapped != null)
                    {
                        for (int index = 0; index < 4; ++index)
                        {
                            SmallSmoke smallSmoke = SmallSmoke.New(captureDuck._trapped.x + Rando.Float(-4f, 4f), captureDuck._trapped.y + Rando.Float(-4f, 4f));
                            smallSmoke.hSpeed += captureDuck._trapped.hSpeed * Rando.Float(0.3f, 0.5f);
                            smallSmoke.vSpeed -= Rando.Float(0.1f, 0.2f);
                            Level.Add((Thing)smallSmoke);
                        }
                    }
                    if (Recorder.currentRecording == null)
                        break;
                    Recorder.currentRecording.LogBonus();
                    break;
            }
        }
    }
}
