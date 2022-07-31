// Decompiled with JetBrains decompiler
// Type: DuckGame.ECrate
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

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
            this._maxHealth = 15f;
            this._hitPoints = 15f;
            this._sprite = new SpriteMap("eCrate", 16, 16);
            this.graphic = _sprite;
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-8f, -8f);
            this.collisionSize = new Vec2(16f, 16f);
            this.depth = -0.5f;
            this._editorName = "E Crate";
            this.editorTooltip = "A mysterious unbreakable crate.";
            this.thickness = 2f;
            this.weight = 5f;
            this.flammable = 0.3f;
            this._holdOffset = new Vec2(2f, 0.0f);
            this._light = new Sprite("eCrateLight");
            this._light.CenterOrigin();
            this.collideSounds.Add("crateHit");
        }

        protected override bool OnDestroy(DestroyType type = null)
        {
            this._hitPoints = 0.0f;
            for (int index = 0; index < 6; ++index)
                Level.Add(new GlassParticle(this.x - 8f + Rando.Float(16f), this.y - 8f + Rando.Float(16f), new Vec2(Rando.Float(-2f, 2f), Rando.Float(-2f, 2f))));
            for (int index = 0; index < 5; ++index)
            {
                SmallSmoke smallSmoke = SmallSmoke.New(this.x + Rando.Float(-6f, 6f), this.y + Rando.Float(-6f, 6f));
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
            if (_hitPoints <= 0.0)
                return false;
            if (bullet.isLocal && this.owner == null)
                Thing.Fondle(this, DuckNetwork.localConnection);
            for (int index = 0; index < 1.0 + damageMultiplier / 2.0; ++index)
                Level.Add(new GlassParticle(hitPos.x, hitPos.y, bullet.travelDirNormalized));
            SFX.Play("woodHit");
            if (this.isServerForObject && TeamSelect2.Enabled("EXPLODEYCRATES"))
            {
                Thing.Fondle(this, DuckNetwork.localConnection);
                if (this.duck != null)
                    this.duck.ThrowItem();
                this.Destroy(new DTShot(bullet));
                Level.Add(new GrenadeExplosion(this.x, this.y));
            }
            this._hitPoints -= this.damageMultiplier;
            this.damageMultiplier += 2f;
            if (_hitPoints <= 0.0)
            {
                if (bullet.isLocal)
                    Thing.SuperFondle(this, DuckNetwork.localConnection);
                this.Destroy(new DTShot(bullet));
            }
            return base.Hit(bullet, hitPos);
        }

        public override void ExitHit(Bullet bullet, Vec2 exitPos)
        {
            for (int index = 0; index < 1f + damageMultiplier / 2f; ++index)
                Level.Add(new GlassParticle(exitPos.x, exitPos.y, -bullet.travelDirNormalized));
        }

        public override void Update()
        {
            this._colorFlux.Update();
            base.Update();
            if (damageMultiplier > 1f)
                this.damageMultiplier -= 0.2f;
            else
                this.damageMultiplier = 1f;
            this._sprite.frame = (int)Math.Floor((1f - _hitPoints / (double)this._maxHealth) * 4f);
            if (_hitPoints <= 0f && !this._destroyed)
                this.Destroy(new DTImpact(this));
            if (!this._onFire || burnt >= 0.9f)
                return;
            float num = 1f - this.burnt;
            if (_hitPoints > num * _maxHealth)
                this._hitPoints = num * this._maxHealth;
            this._sprite.color = new Color(num, num, num);
        }

        public override void Draw()
        {
            base.Draw();
            this._light.depth = this.depth + 1;
            float num1 = ((this._hitPoints / this._maxHealth) * 0.7f + 0.3f);
            float g = 1f;
            float num2 = 1f;
            if (_hitPoints < _maxHealth / 2f)
            {
                g = 0f;
                num2 = 0.4f;
            }
            this._light.color = new Color(1f - g, g, 0.2f) * Maths.Clamp(num2 + this._colorFlux.normalized * (1f - num2), 0f, 1f);
            Graphics.Draw(this._light, this.x, this.y);
        }
    }
}
