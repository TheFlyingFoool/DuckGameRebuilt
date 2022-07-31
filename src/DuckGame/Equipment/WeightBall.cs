// Decompiled with JetBrains decompiler
// Type: DuckGame.WeightBall
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class WeightBall : Holdable, IPlatform
    {
        private PhysicsObject _attach;
        public ChokeCollar collar;
        private List<ChainLink> _links = new List<ChainLink>();
        private bool _isMace;
        private float _sparkWait;

        public WeightBall(float xpos, float ypos, PhysicsObject d, ChokeCollar c, bool isMace)
          : base(xpos, ypos)
        {
            this._attach = d;
            if (isMace)
            {
                this.graphic = new Sprite("maceBall");
                this.center = new Vec2(9f, 9f);
                this._collisionOffset = new Vec2(-8f, -8f);
                this._collisionSize = new Vec2(14f, 14f);
                this._impactThreshold = 4f;
                this.canPickUp = false;
                this.onlyCrush = true;
                this._isMace = true;
            }
            else
            {
                this.graphic = new Sprite("weightBall");
                this.center = new Vec2(8f, 8f);
                this._collisionOffset = new Vec2(-7f, -7f);
                this._collisionSize = new Vec2(14f, 14f);
                this._impactThreshold = 2f;
            }
            this.weight = 9f;
            this.thickness = 6f;
            this.physicsMaterial = PhysicsMaterial.Metal;
            this.collideSounds.Add("rockHitGround2");
            this.collar = c;
            this.tapeable = false;
        }

        public void SetAttach(PhysicsObject a) => this._attach = a;

        public override void Initialize()
        {
            for (int index = 0; index < 8; ++index)
            {
                ChainLink chainLink = new ChainLink(this.x, this.y);
                Level.Add(chainLink);
                this._links.Add(chainLink);
            }
            base.Initialize();
        }

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (with is Duck && this.collar != null && with != null)
            {
                if (with == this.collar.owner || (double)this.totalImpactPower <= 8.0)
                    return;
                if (this.collar.duck != null)
                    RumbleManager.AddRumbleEvent(this.collar.duck.profile, new RumbleEvent(RumbleIntensity.Light, RumbleDuration.Pulse, RumbleFalloff.Short));
                if (!this._isMace)
                    return;
                with.Destroy(new DTCrush(this));
            }
            else
                base.OnSoftImpact(with, from);
        }

        public override void OnSolidImpact(MaterialThing with, ImpactedFrom from)
        {
            if (this.collar != null && this.collar.duck != null && this.collar.duck.profile != null && (double)this.totalImpactPower > 4.0)
                RumbleManager.AddRumbleEvent(this.collar.duck.profile, new RumbleEvent(RumbleIntensity.Light, RumbleDuration.Pulse, RumbleFalloff.Short));
            base.OnSolidImpact(with, from);
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            this.hSpeed += bullet.travelDirNormalized.x;
            this.vSpeed += bullet.travelDirNormalized.y;
            SFX.Play("ricochetSmall", Rando.Float(0.6f, 0.7f), Rando.Float(-0.2f, 0.2f));
            return base.Hit(bullet, hitPos);
        }

        public float Solve(PhysicsObject b1, PhysicsObject b2, float dist)
        {
            Thing thing1 = b1.owner != null ? b1.owner : b1;
            Thing thing2 = b2.owner != null ? b2.owner : b2;
            float num1 = dist;
            Vec2 vec2_1 = b2.position - b1.position;
            float num2 = vec2_1.length;
            if ((double)num2 < 0.0001f)
                num2 = 0.0001f;
            if ((double)num2 < (double)num1)
                return 0.0f;
            Vec2 vec2_2 = vec2_1 * (1f / num2);
            Vec2 vec2_3 = new Vec2(thing1.hSpeed, thing1.vSpeed);
            Vec2 vec2_4 = new Vec2(thing2.hSpeed, thing2.vSpeed);
            double num3 = (double)Vec2.Dot(vec2_4 - vec2_3, vec2_2);
            float num4 = num2 - num1;
            float num5 = 2.5f;
            float num6 = 2.1f;
            if (thing1 is ChainLink && !(thing2 is ChainLink))
            {
                num5 = 10f;
                num6 = 0.0f;
            }
            else if (thing2 is ChainLink && !(thing1 is ChainLink))
            {
                num5 = 0.0f;
                num6 = 10f;
            }
            else if (thing1 is ChainLink && thing2 is ChainLink)
            {
                num5 = 10f;
                num6 = 10f;
            }
            if (thing1 is ChokeCollar)
                num5 = 10f;
            else if (thing2 is ChokeCollar)
                num6 = 10f;
            double num7 = (double)num4;
            float num8 = (float)((num3 + num7) / ((double)num5 + (double)num6));
            Vec2 vec2_5 = vec2_2 * num8;
            Vec2 vec2_6 = vec2_3 + vec2_5 * num5;
            vec2_4 -= vec2_5 * num6;
            thing1.hSpeed = vec2_6.x;
            thing1.vSpeed = vec2_6.y;
            thing2.hSpeed = vec2_4.x;
            thing2.vSpeed = vec2_4.y;
            if (thing1 is ChainLink && (double)(thing2.position - thing1.position).length > (double)num1 * 12.0)
                thing1.position = this.position;
            if (thing2 is ChainLink && (double)(thing2.position - thing1.position).length > (double)num1 * 12.0)
                thing2.position = this.position;
            return num8;
        }

        public override void Update()
        {
            PhysicsObject physicsObject = this._attach;
            if (this._attach is Duck)
            {
                Duck attach = this._attach as Duck;
                physicsObject = attach.ragdoll != null ? attach.ragdoll.part1 : attach;
            }
            if (physicsObject == null)
                return;
            double num1 = (double)this.Solve(this, physicsObject, 30f);
            int num2 = 0;
            PhysicsObject b2 = this;
            foreach (ChainLink link in this._links)
            {
                double num3 = (double)this.Solve(link, b2, 2f);
                b2 = link;
                link.depth = this._attach.depth - 8 - num2;
                ++num2;
            }
            double num4 = (double)this.Solve(physicsObject, b2, 2f);
            base.Update();
            if (_sparkWait > 0.0)
                this._sparkWait -= 0.1f;
            else
                this._sparkWait = 0.0f;
            if (_sparkWait != 0.0 || !this.grounded || (double)Math.Abs(this.hSpeed) <= 1.0)
                return;
            this._sparkWait = 0.25f;
            Level.Add(Spark.New(this.x + ((double)this.hSpeed > 0.0 ? -2f : 2f), this.y + 7f, new Vec2(0.0f, 0.5f)));
        }
    }
}
