using System;

namespace DuckGame
{
    [EditorGroup("Stuff|Props")]
    public class ECrate : Holdable, IPlatform
    {
        private float damageMultiplier = 1f;
        private SpriteMap _sprite;
        private Sprite _light;
        private SinWaveManualUpdate _colorFlux = (SinWaveManualUpdate)0.1f;

        public ECrate(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _maxHealth = 15f;
            _hitPoints = 15f;
            _sprite = new SpriteMap("eCrate", 16, 16);
            graphic = _sprite;
            center = new Vec2(8f, 8f);
            collisionOffset = new Vec2(-8f, -8f);
            collisionSize = new Vec2(16f, 16f);
            depth = -0.5f;
            _editorName = "E Crate";
            editorTooltip = "A mysterious unbreakable crate.";
            thickness = 2f;
            weight = 5f;
            flammable = 0.3f;
            _holdOffset = new Vec2(2f, 0f);
            _light = new Sprite("eCrateLight");
            _light.CenterOrigin();
            collideSounds.Add("crateHit");
        }

        protected override bool OnDestroy(DestroyType type = null)
        {
            _hitPoints = 0f;
            for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 6; ++index)
                Level.Add(new GlassParticle(x - 8f + Rando.Float(16f), y - 8f + Rando.Float(16f), new Vec2(Rando.Float(-2f, 2f), Rando.Float(-2f, 2f))));
            for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 5; ++index)
            {
                SmallSmoke smallSmoke = SmallSmoke.New(x + Rando.Float(-6f, 6f), y + Rando.Float(-6f, 6f));
                smallSmoke.hSpeed += Rando.Float(-0.3f, 0.3f);
                smallSmoke.vSpeed -= Rando.Float(0.1f, 0.2f);
                Level.Add(smallSmoke);
            }
            SFX.Play("crateDestroy");
            Level.Remove(this);
            return true;
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if (_hitPoints <= 0)
                return false;
            if (bullet.isLocal && owner == null)
                Fondle(this, DuckNetwork.localConnection);
            for (int index = 0; index < DGRSettings.ActualParticleMultiplier * (1f + damageMultiplier / 2f); ++index)
                Level.Add(new GlassParticle(hitPos.x, hitPos.y, bullet.travelDirNormalized));
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
            if (_hitPoints <= 0)
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
                Level.Add(new GlassParticle(exitPos.x, exitPos.y, -bullet.travelDirNormalized));
        }

        public override void Update()
        {
            _colorFlux.Update();
            base.Update();
            if (damageMultiplier > 1f)
                damageMultiplier -= 0.2f;
            else
                damageMultiplier = 1f;
            _sprite.frame = (int)Math.Floor((1f - _hitPoints / _maxHealth) * 4f);
            if (_hitPoints <= 0f && !_destroyed)
                Destroy(new DTImpact(this));
            if (!_onFire || burnt >= 0.9f)
                return;
            float num = 1f - burnt;
            if (_hitPoints > num * _maxHealth)
                _hitPoints = num * _maxHealth;
            _sprite.color = new Color(num, num, num);
        }

        public override void Draw()
        {
            base.Draw();
            _light.depth = depth + 1;
            float num1 = ((_hitPoints / _maxHealth) * 0.7f + 0.3f);
            float g = 1f;
            float num2 = 1f;
            if (_hitPoints < _maxHealth / 2f)
            {
                g = 0f;
                num2 = 0.4f;
            }
            _light.color = new Color(1f - g, g, 0.2f) * Maths.Clamp(num2 + _colorFlux.normalized * (1f - num2), 0f, 1f);
            Graphics.Draw(ref _light, x, y);
        }
    }
}
