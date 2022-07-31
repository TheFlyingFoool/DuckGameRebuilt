// Decompiled with JetBrains decompiler
// Type: DuckGame.FireCrackers
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    [EditorGroup("Guns|Misc")]
    [BaggedProperty("isFatal", false)]
    public class FireCrackers : Gun
    {
        private SpriteMap _sprite;
        private int _ammoMax = 8;
        private int burnTime;

        public FireCrackers(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = this._ammoMax;
            this._type = "gun";
            this._sprite = new SpriteMap("fireCrackers", 16, 16);
            this.graphic = _sprite;
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-4f, -4f);
            this.collisionSize = new Vec2(8f, 8f);
            this._barrelOffsetTL = new Vec2(12f, 6f);
            this._fullAuto = true;
            this._fireWait = 1f;
            this._kickForce = 1f;
            this.flammable = 1f;
            this.physicsMaterial = PhysicsMaterial.Paper;
            this.editorTooltip = "Warning: these are dangerous explosives, not a spicy snack treat.";
        }

        public override void Initialize() => base.Initialize();

        public override void Update()
        {
            this._sprite.frame = this._ammoMax - this.ammo;
            if (this.ammo == 0 && this.owner != null && this.isServerForObject)
            {
                if (this.held && this.owner is Duck owner)
                    owner.ThrowItem();
                this.level.RemoveThing(this);
            }
            if (this.onFire && this.infinite.value)
            {
                ++this.burnTime;
                if (this.burnTime % 3 == 0)
                {
                    for (int index = 0; index < 3; ++index)
                    {
                        Firecracker firecracker = new Firecracker(this.barrelPosition.x, this.barrelPosition.y)
                        {
                            hSpeed = Rando.Float(-4f, 4f),
                            vSpeed = Rando.Float(-1f, -6f)
                        };
                        Level.Add(firecracker);
                    }
                    SFX.PlaySynchronized("lightMatch", 0.5f, Rando.Float(0.2f) - 0.4f);
                }
                if (this.burnTime > 120)
                    Level.Remove(this);
            }
            base.Update();
        }

        public override void Draw() => base.Draw();

        public override void OnPressAction()
        {
            if (this.ammo <= 0 || !this.isServerForObject)
                return;
            --this.ammo;
            SFX.PlaySynchronized("lightMatch", 0.5f, Rando.Float(0.2f) - 0.4f);
            if (!(this.owner is Duck owner))
                return;
            float num1 = 0f;
            float num2 = 0f;
            if (owner.inputProfile.Down("LEFT"))
                num1 -= 2f;
            if (owner.inputProfile.Down("RIGHT"))
                num1 += 2f;
            if (owner.inputProfile.Down("UP"))
                num2 -= 2f;
            if (owner.inputProfile.Down("DOWN"))
                num2 += 2f;
            Firecracker firecracker = new Firecracker(this.barrelPosition.x, this.barrelPosition.y);
            if (!owner.crouch)
            {
                firecracker.hSpeed = offDir * Rando.Float(2f, 2.5f) + num1;
                firecracker.vSpeed = num2 - 1f + Rando.Float(-0.2f, 0.8f);
            }
            else
                firecracker.spinAngle = 90f;
            Level.Add(firecracker);
        }

        protected override bool OnBurn(Vec2 firePosition, Thing litBy)
        {
            if (this.isServerForObject)
            {
                if (!this.infinite.value)
                {
                    for (int index = 0; index < Math.Min(this.ammo, this._ammoMax); ++index)
                    {
                        Firecracker firecracker = new Firecracker(this.barrelPosition.x, this.barrelPosition.y)
                        {
                            hSpeed = Rando.Float(-4f, 4f),
                            vSpeed = Rando.Float(-1f, -6f)
                        };
                        Level.Add(firecracker);
                    }
                    Level.Remove(this);
                }
                if (this.owner is Duck owner)
                    owner.ThrowItem();
            }
            return true;
        }

        public override void Fire()
        {
        }
    }
}
