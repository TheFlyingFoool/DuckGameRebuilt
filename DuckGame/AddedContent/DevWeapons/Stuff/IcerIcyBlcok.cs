using System;
using System.Collections.Generic;

namespace DuckGame
{
    [ClientOnly]
    public class IcerIcyBlock : Holdable, IPlatform
    {
        public StateBinding _hitPointsBinding = new StateBinding("_hitPoints");
        public StateBinding _xscaleBinding = new StateBinding("xscale");
        private SpriteMap _sprite;
        private float breakPoints = 15f;
        private float damageMultiplier;
        public InvisibleBlock block;
        public IcerIcyBlock(float xpos, float ypos, float size = 0.5f)
          : base(xpos, ypos)
        {
            
            _sprite = new SpriteMap("iceBlock", 16, 16);
            graphic = _sprite;
            center = new Vec2(8f, 8f);
            collisionOffset = new Vec2(-8f, -8f) * size;
            collisionSize = new Vec2(16f, 16f) * size;
            scale = new Vec2(size);
            depth = -0.5f;
            thickness = 2f;
            weight = 5f;
            buoyancy = 1f;
            _hitPoints = 1f * size * 2;
            impactThreshold = -1f;
            physicsMaterial = PhysicsMaterial.Glass;
            _holdOffset = new Vec2(2f, 0f);
            flammable = 0f;
            collideSounds.Add("glassHit");
            superNonFlammable = true;
        }
        protected override float CalculatePersonalImpactPower(MaterialThing with, ImpactedFrom from) => base.CalculatePersonalImpactPower(with, from) - 1.5f;

        public override void OnSolidImpact(MaterialThing with, ImpactedFrom from)
        {
            if (with is PhysicsObject)
            {
                (with as PhysicsObject).specialFrictionMod = 0.16f;
                (with as PhysicsObject).modFric = true;
            }
            base.OnSolidImpact(with, from);
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if (bullet.isLocal && owner == null)
                Fondle(this, DuckNetwork.localConnection);
            for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 4; ++index)
            {
                GlassParticle glassParticle = new GlassParticle(hitPos.x, hitPos.y, bullet.travelDirNormalized);
                Level.Add(glassParticle);
                glassParticle.hSpeed = (-bullet.travelDirNormalized.x * 2f * (Rando.Float(1f) + 0.3f));
                glassParticle.vSpeed = (-bullet.travelDirNormalized.y * 2f * (Rando.Float(1f) + 0.3f)) - Rando.Float(2f);
                Level.Add(glassParticle);
            }
            SFX.Play("glassHit", 0.6f);
            if (bullet.isLocal && TeamSelect2.Enabled("EXPLODEYCRATES"))
            {
                Fondle(this, DuckNetwork.localConnection);
                if (duck != null)
                    duck.ThrowItem();
                Destroy(new DTShot(bullet));
                Level.Add(new GrenadeExplosion(x, y));
            }
            if (isServerForObject && bullet.isLocal)
            {
                breakPoints -= damageMultiplier;
                damageMultiplier += 2f;
                if (breakPoints <= 0f)
                    Destroy(new DTShot(bullet));
                --vSpeed;
                hSpeed += bullet.travelDirNormalized.x;
                vSpeed += bullet.travelDirNormalized.y;
            }
            return base.Hit(bullet, hitPos);
        }
        protected override bool OnDestroy(DestroyType type = null)
        {
            _hitPoints = 0f;
            Level.Remove(this);
            SFX.Play("glassHit");
            Vec2 hitAngle = Vec2.Zero;
            if (type is DTShot)
                hitAngle = (type as DTShot).bullet.travelDirNormalized;
            for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 8; ++index)
            {
                GlassParticle glassParticle = new GlassParticle(x + Rando.Float(-4f, 4f), y + Rando.Float(-4f, 4f), hitAngle);
                Level.Add(glassParticle);
                glassParticle.hSpeed = (hitAngle.x * 2f * (Rando.Float(1f) + 0.3f));
                glassParticle.vSpeed = (hitAngle.y * 2f * (Rando.Float(1f) + 0.3f)) - Rando.Float(2f);
                Level.Add(glassParticle);
            }
            for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 5; ++index)
            {
                SmallSmoke smallSmoke = SmallSmoke.New(x + Rando.Float(-6f, 6f), y + Rando.Float(-6f, 6f));
                smallSmoke.hSpeed += Rando.Float(-0.3f, 0.3f);
                smallSmoke.vSpeed -= Rando.Float(0.1f, 0.2f);
                Level.Add(smallSmoke);
            }
            return true;
        }

        public override void ExitHit(Bullet bullet, Vec2 exitPos)
        {
            for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 4; ++index)
            {
                GlassParticle glassParticle = new GlassParticle(exitPos.x, exitPos.y, bullet.travelDirNormalized);
                Level.Add(glassParticle);
                glassParticle.hSpeed = (bullet.travelDirNormalized.x * 2f * (Rando.Float(1f) + 0.3f));
                glassParticle.vSpeed = (bullet.travelDirNormalized.y * 2f * (Rando.Float(1f) + 0.3f)) - Rando.Float(2f);
                Level.Add(glassParticle);
            }
        }
        public override void HeatUp(Vec2 location)
        {
            _hitPoints -= 0.01f;
            if (_hitPoints < 0.05f)
            {
                Level.Remove(this);
                _destroyed = true;
                for (int index = 0; index < 16; ++index)
                {
                    FluidData water = Fluid.Water;
                    water.amount = 1f / 1000f;
                    Fluid fluid = new Fluid(x + Rando.Int(-6, 6), y + Rando.Int(-6, 6), Vec2.Zero, water)
                    {
                        hSpeed = (index / 16f - 0.5f) * Rando.Float(0.3f, 0.4f),
                        vSpeed = Rando.Float(-1.5f, 0.5f)
                    };
                    Level.Add(fluid);
                }
            }
            FluidData water1 = Fluid.Water;
            water1.amount = 1f / 1000f * xscale;
            Fluid fluid1 = new Fluid(x + Rando.Int(-6, 6), y + Rando.Int(-6, 6), Vec2.Zero, water1)
            {
                hSpeed = Rando.Float(-0.1f, 0.1f),
                vSpeed = Rando.Float(-0.3f, 0.3f)
            };
            Level.Add(fluid1);
            base.HeatUp(location);
        }
        public override void UpdateMaterial()
        {
        }
        public override void Terminate()
        {
            if (block != null)
            {
                Level.Remove(block);
            }
            base.Terminate();
        }
        public override void Update()
        {
            yscale = xscale;
            if (xscale < 1)
            {
                friction = 0.05f;
                HeatUp(position);
            }
            if (xscale > 1)
            {
                ignoreCollisions = true;
                friction = 0.03f;
                if (xscale > 1.5f) friction = 0.01f;
                canPickUp = false;
                if (block == null)
                {
                    block = new InvisibleBlock(x, y);
                    block.thickness = -5;
                    Level.Add(block);
                    clip.Add(block);
                }
                else
                {
                    block.position = position;
                    block.collisionSize = collisionSize;
                    block.collisionOffset = collisionOffset;

                    IEnumerable<PhysicsObject> list = Level.CheckRectAll<PhysicsObject>(block.topLeft - new Vec2(0, 4), block.bottomRight);
                    foreach (PhysicsObject po in list)
                    {
                        if (po is IcerIcyBlock) continue;
                        if (po.isServerForObject)
                        {
                            po.position += velocity;
                            if (Level.CheckRect<Block>(po.topLeft + new Vec2(2), po.bottomRight - new Vec2(2), block) != null)
                            {
                                po.Destroy(new DTCrush(this));
                            }
                        }
                    }
                    if (Math.Abs(vSpeed) > 0)
                    {
                        IEnumerable<PhysicsObject> unsleeplist = Level.CheckRectAll<PhysicsObject>(block.topLeft - new Vec2(0, 10), new Vec2(block.right, block.top));
                        foreach (PhysicsObject po in unsleeplist)
                        {
                            if (po.isServerForObject)
                            {
                                po.sleeping = false;
                                po.grounded = false;
                            }
                        }
                    }
                }
            }
            base.Update();
            heat = -1f;
            if (damageMultiplier > 1)
            {
                damageMultiplier -= 0.2f;
            }
            else
            {
                damageMultiplier = 1f;
                breakPoints = 15f;
            }

            _sprite.frame = (int)Math.Floor((1f - _hitPoints / 1f) * 5f);
            if (_sprite.frame == 0)
            {
                collisionOffset = new Vec2(-8f, -8f) * xscale;
                collisionSize = new Vec2(16f, 16f) * xscale;
            }
            else if (_sprite.frame == 1)
            {
                collisionOffset = new Vec2(-8f, -7f) * xscale;
                collisionSize = new Vec2(16f, 15f) * xscale;
            }
            else if (_sprite.frame == 2)
            {
                collisionOffset = new Vec2(-7f, -4f) * xscale;
                collisionSize = new Vec2(14f, 11f) * xscale;
            }
            else if (_sprite.frame == 3)
            {
                collisionOffset = new Vec2(-6f, -2f) * xscale;
                collisionSize = new Vec2(12f, 7f) * xscale;
            }
            else
            {
                if (_sprite.frame != 4)
                    return;
                collisionOffset = new Vec2(-6f, -1f) * xscale;
                collisionSize = new Vec2(12f, 5f) * xscale;
            }
        }
    }
}
