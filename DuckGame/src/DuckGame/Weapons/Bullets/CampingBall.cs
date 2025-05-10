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
            _sprite = new SpriteMap("camping_ball", 8, 9);
            graphic = _sprite;
            center = new Vec2(4f, 4f);
            collisionOffset = new Vec2(-4f, -4f);
            collisionSize = new Vec2(8f, 8f);
            depth = -0.5f;
            thickness = 2f;
            weight = 1f;
            bouncy = 0.5f;
            _owner = owner;
            _impactThreshold = 0.01f;
        }

        public override void Update()
        {
            if (Math.Abs(hSpeed) + Math.Abs(vSpeed) > 0.1f)
                angleDegrees = -Maths.PointDirection(Vec2.Zero, new Vec2(hSpeed, vSpeed));
            if (isServerForObject)
            {
                if (grounded && Math.Abs(vSpeed) + Math.Abs(hSpeed) <= 0.2f)
                    alpha -= 0.2f;
                if (alpha <= 0f)
                    Level.Remove(this);
                if (!onFire && Level.CheckRect<SmallFire>(position + new Vec2(-6f, -6f), position + new Vec2(6f, 6f), this) != null)
                    LightOnFire();
            }
            base.Update();
        }

        public void LightOnFire()
        {
            onFire = true;
            Level.Add(SmallFire.New(0f, 0f, 0f, 0f, stick: this, firedFrom: this));
        }

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (Network.isActive && connection != DuckNetwork.localConnection || removeFromLevel)
                return;
            switch (with)
            {
                case Duck t when t.ragdoll == null:
                    if (t._trapped != null)
                        break;
                    t.hSpeed = hSpeed * 0.75f;
                    t.vSpeed = vSpeed * 0.75f;
                    if (t.holdObject != null)
                    {
                        Thing holdObject = t.holdObject;
                        t.ThrowItem();
                        Fondle(holdObject);
                    }
                    Fondle(t);
                    t.GoRagdoll();
                    if (t.ragdoll != null && t.ragdoll.part1 != null && t.ragdoll.part2 != null && t.ragdoll.part3 != null)
                    {
                        Fondle(t.ragdoll);
                        t.ragdoll.connection = connection;
                        t.ragdoll.part1.connection = connection;
                        t.ragdoll.part2.connection = connection;
                        t.ragdoll.part3.connection = connection;
                        t.ragdoll.inSleepingBag = true;
                        t.ragdoll.sleepingBagHealth = 60;
                        if (onFire)
                            t.ragdoll.LightOnFire();
                        for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 4; ++index)
                        {
                            SmallSmoke smallSmoke = SmallSmoke.New(t.ragdoll.x + Rando.Float(-4f, 4f), t.ragdoll.y + Rando.Float(-4f, 4f));
                            smallSmoke.hSpeed += t.ragdoll.hSpeed * Rando.Float(0.3f, 0.5f);
                            smallSmoke.vSpeed -= Rando.Float(0.1f, 0.2f);
                            Level.Add(smallSmoke);
                        }
                    }
                    if (Recorder.currentRecording != null)
                        Recorder.currentRecording.LogBonus();
                    Level.Remove(this);
                    break;
                case RagdollPart ragdollPart when ragdollPart.doll != null && !ragdollPart.doll.inSleepingBag:
                    Fondle(ragdollPart.doll);
                    ragdollPart.doll.inSleepingBag = true;
                    ragdollPart.doll.sleepingBagHealth = 60;
                    if (onFire)
                        ragdollPart.doll.LightOnFire();
                    if (ragdollPart.doll.part2 != null && ragdollPart.doll.part1 != null)
                    {
                        ragdollPart.doll.part2.hSpeed = hSpeed * 0.75f;
                        ragdollPart.doll.part2.vSpeed = vSpeed * 0.75f;
                        ragdollPart.doll.part1.hSpeed = hSpeed * 0.75f;
                        ragdollPart.doll.part1.vSpeed = vSpeed * 0.75f;
                    }
                    for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 4; ++index)
                    {
                        SmallSmoke smallSmoke = SmallSmoke.New(ragdollPart.doll.x + Rando.Float(-4f, 4f), ragdollPart.doll.y + Rando.Float(-4f, 4f));
                        smallSmoke.hSpeed += ragdollPart.doll.hSpeed * Rando.Float(0.3f, 0.5f);
                        smallSmoke.vSpeed -= Rando.Float(0.1f, 0.2f);
                        Level.Add(smallSmoke);
                    }
                    if (Recorder.currentRecording != null)
                        Recorder.currentRecording.LogBonus();
                    Level.Remove(this);
                    break;
            }
        }
    }
}
