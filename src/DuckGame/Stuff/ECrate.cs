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
            this.graphic = (Sprite)this._sprite;
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-8f, -8f);
            this.collisionSize = new Vec2(16f, 16f);
            this.depth = - 0.5f;
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
                Level.Add((Thing)new GlassParticle(this.x - 8f + Rando.Float(16f), this.y - 8f + Rando.Float(16f), new Vec2(Rando.Float(-2f, 2f), Rando.Float(-2f, 2f))));
            for (int index = 0; index < 5; ++index)
            {
                SmallSmoke smallSmoke = SmallSmoke.New(this.x + Rando.Float(-6f, 6f), this.y + Rando.Float(-6f, 6f));
                smallSmoke.hSpeed += Rando.Float(-0.3f, 0.3f);
                smallSmoke.vSpeed -= Rando.Float(0.1f, 0.2f);
                Level.Add((Thing)smallSmoke);
            }
            SFX.Play("crateDestroy");
            Level.Remove((Thing)this);
            return true;
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if ((double)this._hitPoints <= 0.0)
                return false;
            if (bullet.isLocal && this.owner == null)
                Thing.Fondle((Thing)this, DuckNetwork.localConnection);
            for (int index = 0; (double)index < 1.0 + (double)this.damageMultiplier / 2.0; ++index)
                Level.Add((Thing)new GlassParticle(hitPos.x, hitPos.y, bullet.travelDirNormalized));
            SFX.Play("woodHit");
            if (this.isServerForObject && TeamSelect2.Enabled("EXPLODEYCRATES"))
            {
                Thing.Fondle((Thing)this, DuckNetwork.localConnection);
                if (this.duck != null)
                    this.duck.ThrowItem();
                this.Destroy((DestroyType)new DTShot(bullet));
                Level.Add((Thing)new GrenadeExplosion(this.x, this.y));
            }
            this._hitPoints -= this.damageMultiplier;
            this.damageMultiplier += 2f;
            if ((double)this._hitPoints <= 0.0)
            {
                if (bullet.isLocal)
                    Thing.SuperFondle((Thing)this, DuckNetwork.localConnection);
                this.Destroy((DestroyType)new DTShot(bullet));
            }
            return base.Hit(bullet, hitPos);
        }

        public override void ExitHit(Bullet bullet, Vec2 exitPos)
        {
            for (int index = 0; (double)index < 1.0 + (double)this.damageMultiplier / 2.0; ++index)
                Level.Add((Thing)new GlassParticle(exitPos.x, exitPos.y, -bullet.travelDirNormalized));
        }

        public override void Update()
        {
            this._colorFlux.Update();
            base.Update();
            if ((double)this.damageMultiplier > 1.0)
                this.damageMultiplier -= 0.2f;
            else
                this.damageMultiplier = 1f;
            this._sprite.frame = (int)Math.Floor((1.0 - (double)this._hitPoints / (double)this._maxHealth) * 4.0);
            if ((double)this._hitPoints <= 0.0 && !this._destroyed)
                this.Destroy((DestroyType)new DTImpact((Thing)this));
            if (!this._onFire || (double)this.burnt >= 0.899999976158142)
                return;
            float num = 1f - this.burnt;
            if ((double)this._hitPoints > (double)num * (double)this._maxHealth)
                this._hitPoints = num * this._maxHealth;
            this._sprite.color = new Color(num, num, num);
        }

        public override void Draw()
        {
            base.Draw();
            this._light.depth = this.depth + 1;
            float num1 = (float)((double)(this._hitPoints / this._maxHealth) * 0.699999988079071 + 0.300000011920929);
            float g = 1f;
            float num2 = 1f;
            if ((double)this._hitPoints < (double)this._maxHealth / 2.0)
            {
                g = 0.0f;
                num2 = 0.4f;
            }
            this._light.color = new Color(1f - g, g, 0.2f) * Maths.Clamp(num2 + this._colorFlux.normalized * (1f - num2), 0.0f, 1f);
            Graphics.Draw(this._light, this.x, this.y);
        }
    }
}
