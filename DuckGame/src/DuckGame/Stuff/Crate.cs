// Decompiled with JetBrains decompiler
// Type: DuckGame.Crate
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    [EditorGroup("Stuff|Props")]
    [BaggedProperty("isInDemo", true)]
    [BaggedProperty("previewPriority", true)]
    public class Crate : Holdable, IPlatform
    {
        public StateBinding _destroyedBinding = new StateBinding("_destroyed");
        public StateBinding _hitPointsBinding = new StateBinding("_hitPoints");
        public StateBinding _damageMultiplierBinding = new StateBinding(nameof(damageMultiplier));
        public float damageMultiplier = 1f;
        private SpriteMap _sprite;
        private float _burnt;

        public Crate(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _maxHealth = 15f;
            _hitPoints = 15f;
            _sprite = new SpriteMap("crate", 16, 16);
            graphic = _sprite;
            center = new Vec2(8f, 8f);
            collisionOffset = new Vec2(-8f, -8f);
            collisionSize = new Vec2(16f, 16f);
            depth = -0.5f;
            _editorName = nameof(Crate);
            thickness = 2f;
            weight = 5f;
            buoyancy = 1f;
            _holdOffset = new Vec2(2f, 0f);
            flammable = 0.3f;
            collideSounds.Add("crateHit");
            editorTooltip = "It's made of wood. That's...pretty much it.";
        }

        protected override bool OnDestroy(DestroyType type = null)
        {
            _hitPoints = 0f;
            Level.Remove(this);
            SFX.Play("crateDestroy");
            Vec2 vec2 = Vec2.Zero;
            if (type is DTShot)
                vec2 = (type as DTShot).bullet.travelDirNormalized;
            for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 6; ++index)
            {
                WoodDebris woodDebris = WoodDebris.New(x - 8f + Rando.Float(16f), y - 8f + Rando.Float(16f));
                woodDebris.hSpeed = (float)((Rando.Float(1f) > 0.5 ? 1.0 : -1.0) * Rando.Float(3f) + Math.Sign(vec2.x) * 0.5);
                woodDebris.vSpeed = -Rando.Float(1f);
                Level.Add(woodDebris);
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

        private bool CheckForPhysicalBullet(MaterialThing with)
        {
            if (with is PhysicalBullet)
            {
                Bullet bullet = (with as PhysicalBullet).bullet;
                if (bullet != null && bullet.ammo is ATGrenade)
                    return true;
            }
            return false;
        }

        public override void SolidImpact(MaterialThing with, ImpactedFrom from)
        {
            if (CheckForPhysicalBullet(with))
                Destroy(new DTShot((with as PhysicalBullet).bullet));
            else
                base.SolidImpact(with, from);
        }

        public override void Impact(MaterialThing with, ImpactedFrom from, bool solidImpact)
        {
            if (CheckForPhysicalBullet(with))
                Destroy(new DTShot((with as PhysicalBullet).bullet));
            else
                base.Impact(with, from, solidImpact);
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if (_hitPoints <= 0.0)
                return base.Hit(bullet, hitPos);
            if (bullet.isLocal && owner == null)
                Fondle(this, DuckNetwork.localConnection);
            for (int index = 0; index < DGRSettings.ActualParticleMultiplier * (1f + damageMultiplier / 2f); ++index)
            {
                WoodDebris woodDebris = WoodDebris.New(hitPos.x, hitPos.y);
                woodDebris.hSpeed = -bullet.travelDirNormalized.x * 2f * (Rando.Float(1f) + 0.3f);
                woodDebris.vSpeed = (-bullet.travelDirNormalized.y * 2f * (Rando.Float(1f) + 0.3f)) - Rando.Float(2f);
                Level.Add(woodDebris);
            }
            SFX.Play("woodHit");
            if (isServerForObject && TeamSelect2.Enabled("EXPLODEYCRATES"))
            {
                Fondle(this, DuckNetwork.localConnection);
                if (duck != null)
                    duck.ThrowItem();
                Destroy(new DTShot(bullet));
                Level.Add(new GrenadeExplosion(x, y));
            }
            _hitPoints -= damageMultiplier;
            damageMultiplier += 2f;
            if (_hitPoints <= 0.0)
            {
                if (bullet.isLocal)
                    SuperFondle(this, DuckNetwork.localConnection);
                Destroy(new DTShot(bullet));
            }
            return base.Hit(bullet, hitPos);
        }

        public override void ExitHit(Bullet bullet, Vec2 exitPos)
        {
            for (int index = 0; index < DGRSettings.ActualParticleMultiplier * (1f + damageMultiplier / 2f); ++index)
            {
                WoodDebris woodDebris = WoodDebris.New(exitPos.x, exitPos.y);
                woodDebris.hSpeed = (bullet.travelDirNormalized.x * 3f * (Rando.Float(1f) + 0.3f));
                woodDebris.vSpeed = (bullet.travelDirNormalized.y * 3f * (Rando.Float(1f) + 0.3f) - (Rando.Float(2f) - 1f));
                Level.Add(woodDebris);
            }
        }

        public override void Update()
        {
            base.Update();
            if (damageMultiplier > 1f)
                damageMultiplier -= 0.2f;
            else
                damageMultiplier = 1f;
            _sprite.frame = (int)Math.Floor((1f - _hitPoints / _maxHealth) * 4f);
            if (_hitPoints <= 0f && !_destroyed)
                Destroy(new DTImpact(this));
            if (!_onFire || _burnt >= 0.9f)
                return;
            float num = 1f - burnt;
            if (_hitPoints > num * _maxHealth)
                _hitPoints = num * _maxHealth;
            _sprite.color = new Color(num, num, num);
        }
    }
}
