// Decompiled with JetBrains decompiler
// Type: DuckGame.WeightBall
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            _attach = d;
            if (isMace)
            {
                graphic = new Sprite("maceBall");
                center = new Vec2(9f, 9f);
                _collisionOffset = new Vec2(-8f, -8f);
                _collisionSize = new Vec2(14f, 14f);
                _impactThreshold = 4f;
                canPickUp = false;
                onlyCrush = true;
                _isMace = true;
            }
            else
            {
                graphic = new Sprite("weightBall");
                center = new Vec2(8f, 8f);
                _collisionOffset = new Vec2(-7f, -7f);
                _collisionSize = new Vec2(14f, 14f);
                _impactThreshold = 2f;
            }
            weight = 9f;
            thickness = 6f;
            physicsMaterial = PhysicsMaterial.Metal;
            collideSounds.Add("rockHitGround2");
            collar = c;
            tapeable = false;
        }

        public void SetAttach(PhysicsObject a) => _attach = a;

        public override void Initialize()
        {
            for (int index = 0; index < 8; ++index)
            {
                ChainLink chainLink = new ChainLink(x, y);
                Level.Add(chainLink);
                _links.Add(chainLink);
            }
            base.Initialize();
        }

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (with is Duck && collar != null && with != null)
            {
                if (with == collar.owner || totalImpactPower <= 8f) return;
                if (collar.duck != null) RumbleManager.AddRumbleEvent(collar.duck.profile, new RumbleEvent(RumbleIntensity.Light, RumbleDuration.Pulse, RumbleFalloff.Short));
                if (!_isMace) return;
                with.Destroy(new DTCrush(this));
            }
            else base.OnSoftImpact(with, from);
        }

        public override void OnSolidImpact(MaterialThing with, ImpactedFrom from)
        {
            if (collar != null && collar.duck != null && collar.duck.profile != null && totalImpactPower > 4f) RumbleManager.AddRumbleEvent(collar.duck.profile, new RumbleEvent(RumbleIntensity.Light, RumbleDuration.Pulse, RumbleFalloff.Short));
            base.OnSolidImpact(with, from);
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            hSpeed += bullet.travelDirNormalized.x;
            vSpeed += bullet.travelDirNormalized.y;
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
            if (num2 < 0.0001f)
                num2 = 0.0001f;
            if (num2 < num1)
                return 0f;
            Vec2 vec2_2 = vec2_1 * (1f / num2);
            Vec2 vec2_3 = new Vec2(thing1.hSpeed, thing1.vSpeed);
            Vec2 vec2_4 = new Vec2(thing2.hSpeed, thing2.vSpeed);
            double num3 = Vec2.Dot(vec2_4 - vec2_3, vec2_2);
            float num4 = num2 - num1;
            float num5 = 2.5f;
            float num6 = 2.1f;
            if (thing1 is ChainLink && !(thing2 is ChainLink))
            {
                num5 = 10f;
                num6 = 0f;
            }
            else if (thing2 is ChainLink && !(thing1 is ChainLink))
            {
                num5 = 0f;
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
            double num7 = num4;
            float num8 = (float)((num3 + num7) / (num5 + num6));
            Vec2 vec2_5 = vec2_2 * num8;
            Vec2 vec2_6 = vec2_3 + vec2_5 * num5;
            vec2_4 -= vec2_5 * num6;
            thing1.hSpeed = vec2_6.x;
            thing1.vSpeed = vec2_6.y;
            thing2.hSpeed = vec2_4.x;
            thing2.vSpeed = vec2_4.y;
            if (thing1 is ChainLink && (thing2.position - thing1.position).length > num1 * 12f) thing1.position = position;
            if (thing2 is ChainLink && (thing2.position - thing1.position).length > num1 * 12f) thing2.position = position;
            return num8;
        }

        public override void Update()
        {
            PhysicsObject physicsObject = _attach;
            if (_attach is Duck)
            {
                Duck attach = _attach as Duck;
                physicsObject = attach.ragdoll != null ? attach.ragdoll.part1 : attach;
            }
            if (physicsObject == null) return;
            double num1 = Solve(this, physicsObject, 30f);
            int num2 = 0;
            PhysicsObject b2 = this;
            foreach (ChainLink link in _links)
            {
                double num3 = Solve(link, b2, 2f);
                b2 = link;
                link.depth = _attach.depth - 8 - num2;
                ++num2;
            }
            //double num4 = Solve(physicsObject, b2, 2f); what -NiK0
            base.Update();
            if (_sparkWait > 0f) _sparkWait -= 0.1f;
            else _sparkWait = 0f;
            if (_sparkWait != 0f || !grounded || Math.Abs(hSpeed) <= 1f) return;
            _sparkWait = 0.25f;
            if (DGRSettings.S_ParticleMultiplier >= 1) for (int i = 0; i < DGRSettings.S_ParticleMultiplier; i++) Level.Add(Spark.New(x + (hSpeed > 0f ? -2f : 2f), y + 7f, new Vec2(0f, 0.5f)));
            else if (Rando.Int(DGRSettings.S_ParticleMultiplier) > 0) Level.Add(Spark.New(x + (hSpeed > 0f ? -2f : 2f), y + 7f, new Vec2(0f, 0.5f)));
        }
    }
}
