// Decompiled with JetBrains decompiler
// Type: DuckGame.FlareGun
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Guns|Fire")]
    public class FlareGun : Gun
    {
        public FlareGun(float xval, float yval)
          : base(xval, yval)
        {
            ammo = 2;
            _ammoType = new AT9mm
            {
                combustable = true
            };
            wideBarrel = true;
            _type = "gun";
            graphic = new Sprite("flareGun");
            center = new Vec2(8f, 8f);
            collisionOffset = new Vec2(-8f, -4f);
            collisionSize = new Vec2(16f, 9f);
            _barrelOffsetTL = new Vec2(18f, 6f);
            _fullAuto = true;
            _fireWait = 1f;
            _kickForce = 1f;
            _fireRumble = RumbleIntensity.Kick;
            _barrelAngleOffset = 4f;
            _editorName = "Flare Gun";
            editorTooltip = "Shoots a flare at long range that spits fire on impact. Fun at parties!";
            _bio = "For safety purposes, used to call help. What? No it's not a weapon. NO DON'T USE IT LIKE THAT!";
        }
        public override Holdable BecomeTapedMonster(TapedGun pTaped)
        {
            if (Editor.clientonlycontent)
            {
                return pTaped.gun1 is FlareGun && pTaped.gun2 is FlameThrower ? new FlareFlameThrower(x, y) : null;
            }
            return base.BecomeTapedMonster(pTaped);
        }
        public override void Initialize() => base.Initialize();

        public override void Update() => base.Update();

        public override void Draw() => base.Draw();

        public override void OnPressAction()
        {
            if (ammo > 0)
            {
                --ammo;
                _barrelHeat += 0.15f;
                SFX.Play("netGunFire", 0.5f, Rando.Float(0.2f) - 0.4f);
                if (duck != null)
                    RumbleManager.AddRumbleEvent(duck.profile, new RumbleEvent(_fireRumble, RumbleDuration.Pulse, RumbleFalloff.None));
                ApplyKick();
                if (receivingPress || !isServerForObject)
                    return;
                Vec2 vec2 = Offset(barrelOffset);
                Flare t = new Flare(vec2.x, vec2.y, this);
                Fondle(t);
                Vec2 vec = Maths.AngleToVec(barrelAngle + Rando.Float(-0.2f, 0.2f));
                t.hSpeed = vec.x * 14f;
                t.vSpeed = vec.y * 14f;
                Level.Add(t);
            }
            else
                DoAmmoClick();
        }

        public override void Fire()
        {
        }
    }
}
