// Decompiled with JetBrains decompiler
// Type: DuckGame.FlameThrower
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Guns|Fire")]
    [BaggedProperty("isSuperWeapon", true)]
    public class FlameThrower : Gun
    {
        public StateBinding _firingBinding = new StateBinding(nameof(_firing));
        private SpriteMap _barrelFlame;
        public bool _firing;
        private new float _flameWait;
        private SpriteMap _can;
        private ConstantSound _sound = new ConstantSound("flameThrowing");
        private int _maxAmmo = 100;

        public FlameThrower(float xval, float yval)
          : base(xval, yval)
        {
            this.barrelInsertOffset = new Vec2(0f, -2f);
            this.wideBarrel = true;
            this.ammo = this._maxAmmo;
            this._ammoType = new AT9mm();
            this._ammoType.combustable = true;
            this._type = "gun";
            this.graphic = new Sprite("flamethrower");
            this.center = new Vec2(16f, 15f);
            this.collisionOffset = new Vec2(-8f, -3f);
            this.collisionSize = new Vec2(16f, 9f);
            this._barrelOffsetTL = new Vec2(28f, 16f);
            this._fireSound = "smg";
            this._fullAuto = true;
            this._fireWait = 1f;
            this._kickForce = 1f;
            this._barrelFlame = new SpriteMap("flameBurst", 20, 21)
            {
                center = new Vec2(0f, 17f)
            };
            this._barrelFlame.AddAnimation("idle", 0.4f, true, 0, 1, 2, 3);
            this._barrelFlame.AddAnimation("puff", 0.4f, false, 4, 5, 6, 7);
            this._barrelFlame.AddAnimation("flame", 0.4f, true, 8, 9, 10, 11);
            this._barrelFlame.AddAnimation("puffOut", 0.4f, false, 12, 13, 14, 15);
            this._barrelFlame.SetAnimation("idle");
            this._can = new SpriteMap("flamethrowerCan", 8, 8)
            {
                center = new Vec2(4f, 4f)
            };
            this._holdOffset = new Vec2(2f, 0f);
            this._barrelAngleOffset = 8f;
            this._editorName = "Flame Thrower";
            this.editorTooltip = "Some Ducks just want to watch the world burn.";
            this._bio = "I have a problem. I want this flame here, to be over there. But I can't pick it up, it's too damn hot. If only there was some way I could throw it.";
        }

        public override void Update()
        {
            base.Update();
            if (this.ammo == 0)
            {
                this._firing = false;
                this._barrelFlame.speed = 0f;
            }
            if (this._firing && this._barrelFlame.currentAnimation == "idle")
                this._barrelFlame.SetAnimation("puff");
            if (this._firing && this._barrelFlame.currentAnimation == "puff" && this._barrelFlame.finished)
                this._barrelFlame.SetAnimation("flame");
            if (!this._firing && this._barrelFlame.currentAnimation != "idle")
                this._barrelFlame.SetAnimation("puffOut");
            if (this._barrelFlame.currentAnimation == "puffOut" && this._barrelFlame.finished)
                this._barrelFlame.SetAnimation("idle");
            this._sound.lerpVolume = this._firing ? 0.5f : 0f;
            if (this.isServerForObject && this._firing && this._barrelFlame.imageIndex > 5)
            {
                this._flameWait -= 0.25f;
                if (_flameWait > 0.0)
                    return;
                Vec2 vec = Maths.AngleToVec(this.barrelAngle + Rando.Float(-0.5f, 0.5f));
                Vec2 vec2 = new Vec2(vec.x * Rando.Float(2f, 3.5f), vec.y * Rando.Float(2f, 3.5f));
                this.ammo -= 2;
                Level.Add(SmallFire.New(this.barrelPosition.x, this.barrelPosition.y, vec2.x, vec2.y, firedFrom: this));
                this._flameWait = 1f;
            }
            else
                this._flameWait = 0f;
        }

        public override void Draw()
        {
            base.Draw();
            Material material = Graphics.material;
            Graphics.material = null;
            if ((double)this._barrelFlame.speed > 0.0)
            {
                this._barrelFlame.alpha = 0.9f;
                this.Draw(_barrelFlame, new Vec2(11f, 1f));
            }
            this._can.frame = (int)((1.0 - ammo / (double)this._maxAmmo) * 15.0);
            this.Draw(_can, new Vec2(this.barrelOffset.x - 11f, this.barrelOffset.y + 4f));
            Graphics.material = material;
        }

        public override void OnPressAction()
        {
            if (heat > 1.0)
            {
                for (int index = 0; index < this.ammo / 10 + 3; ++index)
                    Level.Add(SmallFire.New(this.x - 6f + Rando.Float(12f), this.y - 8f + Rando.Float(4f), Rando.Float(6f) - 3f, 1f - Rando.Float(4.5f), firedFrom: this));
                SFX.Play("explode", pitch: (Rando.Float(0.3f) - 0.3f));
                Level.Remove(this);
                this._sound.Kill();
                Level.Add(new ExplosionPart(this.x, this.y));
            }
            this._firing = true;
        }

        public override void OnReleaseAction() => this._firing = false;

        public override void Fire()
        {
        }
    }
}
