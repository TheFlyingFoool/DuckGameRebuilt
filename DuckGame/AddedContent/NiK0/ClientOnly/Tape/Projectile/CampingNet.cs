using System;

namespace DuckGame
{
    [ClientOnly]
    public class CampingNet : PhysicsObject
    {
        public CampingNet(float xpos, float ypos)
          : base(xpos, ypos)
        {
            graphic = new Sprite("campnet");
            center = new Vec2(5.5f, 5.5f);
            collisionOffset = new Vec2(-6f, -5f);
            collisionSize = new Vec2(12f, 12f);
            depth = -0.5f;
            thickness = 3f;
            weight = 1f; 
            bouncy = 0.4f;
            _impactThreshold = 0.01f;
            hMax = 14;
            vMax = 12;
        }

        public override void Update()
        {
            if (Math.Abs(hSpeed) + Math.Abs(vSpeed) > 0.1f)
                angle = -Maths.PointDirectionRad(Vec2.Zero, velocity);
            if (grounded && Math.Abs(vSpeed) + Math.Abs(hSpeed) <= 0f)
                alpha -= 0.1f;
            if (alpha <= 0f)
                Level.Remove(this);
            if (!onFire && Level.CheckRect<SmallFire>(position + new Vec2(-5f, -5f), position + new Vec2(5f, 5f), this) != null)
            {
                onFire = true;
                Level.Add(SmallFire.New(0f, 0f, 0f, 0f, stick: this, firedFrom: this));
            }
            base.Update();
        }
        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (Network.isActive && connection != DuckNetwork.localConnection || removeFromLevel)
                return;
            switch (with)
            {
                case Duck t when t.ragdoll == null:
                    t.hSpeed = hSpeed;
                    t.vSpeed = vSpeed;
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
                        t.ragdoll.sleepingBagHealth = 110;
                        Level.Add(new CampNetRagdoll(t.ragdoll.x, t.ragdoll.y) { attatchedTo = t.ragdoll });
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
                    Level.Remove(this);
                    break;
                case RagdollPart ragdollPart when ragdollPart.doll != null && !ragdollPart.doll.inSleepingBag:
                    Fondle(ragdollPart.doll);
                    ragdollPart.doll.inSleepingBag = true;
                    ragdollPart.doll.sleepingBagHealth = 110;
                    if (onFire)
                        ragdollPart.doll.LightOnFire();
                    if (ragdollPart.doll.part2 != null && ragdollPart.doll.part1 != null)
                    {
                        ragdollPart.doll.part2.hSpeed = hSpeed;
                        ragdollPart.doll.part2.vSpeed = vSpeed;
                        ragdollPart.doll.part1.hSpeed = hSpeed;
                        ragdollPart.doll.part1.vSpeed = vSpeed;
                    }
                    Level.Add(new CampNetRagdoll(ragdollPart.doll.x, ragdollPart.doll.y) { attatchedTo = ragdollPart.doll });
                    for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 4; ++index)
                    {
                        SmallSmoke smallSmoke = SmallSmoke.New(ragdollPart.doll.x + Rando.Float(-4f, 4f), ragdollPart.doll.y + Rando.Float(-4f, 4f));
                        smallSmoke.hSpeed += ragdollPart.doll.hSpeed * Rando.Float(0.3f, 0.5f);
                        smallSmoke.vSpeed -= Rando.Float(0.1f, 0.2f);
                        Level.Add(smallSmoke);
                    }
                    Level.Remove(this);
                    break;
            }
        }
    }
}
