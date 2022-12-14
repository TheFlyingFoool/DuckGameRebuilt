// Decompiled with JetBrains decompiler
// Type: DuckGame.FireCrackers
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            ammo = _ammoMax;
            _type = "gun";
            _sprite = new SpriteMap("fireCrackers", 16, 16);
            graphic = _sprite;
            center = new Vec2(8f, 8f);
            collisionOffset = new Vec2(-4f, -4f);
            collisionSize = new Vec2(8f, 8f);
            _barrelOffsetTL = new Vec2(12f, 6f);
            _fullAuto = true;
            _fireWait = 1f;
            _kickForce = 1f;
            flammable = 1f;
            physicsMaterial = PhysicsMaterial.Paper;
            editorTooltip = "Warning: these are dangerous explosives, not a spicy snack treat.";
        }

        public override void Initialize() => base.Initialize();

        public override void Update()
        {
            _sprite.frame = _ammoMax - ammo;
            if (ammo == 0 && owner != null && isServerForObject)
            {
                if (held && this.owner is Duck owner)
                    owner.ThrowItem();
                level.RemoveThing(this);
            }
            if (onFire && infinite.value)
            {
                ++burnTime;
                if (burnTime % 3 == 0)
                {
                    for (int index = 0; index < 3; ++index)
                    {
                        Firecracker firecracker = new Firecracker(barrelPosition.x, barrelPosition.y)
                        {
                            hSpeed = Rando.Float(-4f, 4f),
                            vSpeed = Rando.Float(-1f, -6f)
                        };
                        Level.Add(firecracker);
                    }
                    SFX.PlaySynchronized("lightMatch", 0.5f, Rando.Float(0.2f) - 0.4f);
                }
                if (burnTime > 120)
                    Level.Remove(this);
            }
            base.Update();
        }

        public override void Draw() => base.Draw();

        public override void OnPressAction()
        {
            if (ammo <= 0 || !isServerForObject)
                return;
            --ammo;
            SFX.PlaySynchronized("lightMatch", 0.5f, Rando.Float(0.2f) - 0.4f);
            if (!(this.owner is Duck owner))
                return;
            float num1 = 0f;
            float num2 = 0f;
            if (owner.inputProfile.Down(Triggers.Left))
                num1 -= 2f;
            if (owner.inputProfile.Down(Triggers.Right))
                num1 += 2f;
            if (owner.inputProfile.Down(Triggers.Up))
                num2 -= 2f;
            if (owner.inputProfile.Down(Triggers.Down))
                num2 += 2f;
            Firecracker firecracker = new Firecracker(barrelPosition.x, barrelPosition.y);
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
            if (isServerForObject)
            {
                if (!infinite.value)
                {
                    for (int index = 0; index < Math.Min(ammo, _ammoMax); ++index)
                    {
                        Firecracker firecracker = new Firecracker(barrelPosition.x, barrelPosition.y)
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
