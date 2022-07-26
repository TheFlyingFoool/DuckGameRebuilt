// Decompiled with JetBrains decompiler
// Type: DuckGame.CampingBall
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public class CampingBall : PhysicsObject
    {
        private SpriteMap _sprite;
        private Duck _owner;

        public CampingBall(float xpos, float ypos, Duck owner)
          : base(xpos, ypos)
        {
            this._sprite = new SpriteMap("camping_ball", 8, 9);
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(4f, 4f);
            this.collisionOffset = new Vec2(-4f, -4f);
            this.collisionSize = new Vec2(8f, 8f);
            this.depth = - 0.5f;
            this.thickness = 2f;
            this.weight = 1f;
            this.bouncy = 0.5f;
            this._owner = owner;
            this._impactThreshold = 0.01f;
        }

        public override void Update()
        {
            if ((double)Math.Abs(this.hSpeed) + (double)Math.Abs(this.vSpeed) > 0.100000001490116)
                this.angleDegrees = -Maths.PointDirection(Vec2.Zero, new Vec2(this.hSpeed, this.vSpeed));
            if (this.isServerForObject)
            {
                if (this.grounded && (double)Math.Abs(this.vSpeed) + (double)Math.Abs(this.hSpeed) <= 0.200000002980232)
                    this.alpha -= 0.2f;
                if ((double)this.alpha <= 0.0)
                    Level.Remove((Thing)this);
                if (!this.onFire && Level.CheckRect<SmallFire>(this.position + new Vec2(-6f, -6f), this.position + new Vec2(6f, 6f), (Thing)this) != null)
                    this.LightOnFire();
            }
            base.Update();
        }

        public void LightOnFire()
        {
            this.onFire = true;
            Level.Add((Thing)SmallFire.New(0.0f, 0.0f, 0.0f, 0.0f, stick: ((MaterialThing)this), firedFrom: ((Thing)this)));
        }

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (Network.isActive && this.connection != DuckNetwork.localConnection || this.removeFromLevel)
                return;
            switch (with)
            {
                case Duck t when t.ragdoll == null:
                    if (t._trapped != null)
                        break;
                    t.hSpeed = this.hSpeed * 0.75f;
                    t.vSpeed = this.vSpeed * 0.75f;
                    if (t.holdObject != null)
                    {
                        Thing holdObject = (Thing)t.holdObject;
                        t.ThrowItem();
                        this.Fondle(holdObject);
                    }
                    this.Fondle((Thing)t);
                    t.GoRagdoll();
                    if (t.ragdoll != null && t.ragdoll.part1 != null && t.ragdoll.part2 != null && t.ragdoll.part3 != null)
                    {
                        this.Fondle((Thing)t.ragdoll);
                        t.ragdoll.connection = this.connection;
                        t.ragdoll.part1.connection = this.connection;
                        t.ragdoll.part2.connection = this.connection;
                        t.ragdoll.part3.connection = this.connection;
                        t.ragdoll.inSleepingBag = true;
                        t.ragdoll.sleepingBagHealth = (byte)60;
                        if (this.onFire)
                            t.ragdoll.LightOnFire();
                        for (int index = 0; index < 4; ++index)
                        {
                            SmallSmoke smallSmoke = SmallSmoke.New(t.ragdoll.x + Rando.Float(-4f, 4f), t.ragdoll.y + Rando.Float(-4f, 4f));
                            smallSmoke.hSpeed += t.ragdoll.hSpeed * Rando.Float(0.3f, 0.5f);
                            smallSmoke.vSpeed -= Rando.Float(0.1f, 0.2f);
                            Level.Add((Thing)smallSmoke);
                        }
                    }
                    if (Recorder.currentRecording != null)
                        Recorder.currentRecording.LogBonus();
                    Level.Remove((Thing)this);
                    break;
                case RagdollPart ragdollPart when ragdollPart.doll != null && !ragdollPart.doll.inSleepingBag:
                    this.Fondle((Thing)ragdollPart.doll);
                    ragdollPart.doll.inSleepingBag = true;
                    ragdollPart.doll.sleepingBagHealth = (byte)60;
                    if (this.onFire)
                        ragdollPart.doll.LightOnFire();
                    if (ragdollPart.doll.part2 != null && ragdollPart.doll.part1 != null)
                    {
                        ragdollPart.doll.part2.hSpeed = this.hSpeed * 0.75f;
                        ragdollPart.doll.part2.vSpeed = this.vSpeed * 0.75f;
                        ragdollPart.doll.part1.hSpeed = this.hSpeed * 0.75f;
                        ragdollPart.doll.part1.vSpeed = this.vSpeed * 0.75f;
                    }
                    for (int index = 0; index < 4; ++index)
                    {
                        SmallSmoke smallSmoke = SmallSmoke.New(ragdollPart.doll.x + Rando.Float(-4f, 4f), ragdollPart.doll.y + Rando.Float(-4f, 4f));
                        smallSmoke.hSpeed += ragdollPart.doll.hSpeed * Rando.Float(0.3f, 0.5f);
                        smallSmoke.vSpeed -= Rando.Float(0.1f, 0.2f);
                        Level.Add((Thing)smallSmoke);
                    }
                    if (Recorder.currentRecording != null)
                        Recorder.currentRecording.LogBonus();
                    Level.Remove((Thing)this);
                    break;
            }
        }
    }
}
