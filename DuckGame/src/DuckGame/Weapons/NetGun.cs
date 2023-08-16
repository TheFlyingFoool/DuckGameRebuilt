// Decompiled with JetBrains decompiler
// Type: DuckGame.NetGun
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    [EditorGroup("Guns|Misc")]
    [BaggedProperty("isFatal", false)]
    [BaggedProperty("isInDemo", true)]
    [BaggedProperty("previewPriority", true)]
    public class NetGun : Gun
    {
        private SpriteMap _barrelSteam;
        private SpriteMap _netGunGuage;

        public NetGun(float xval, float yval)
          : base(xval, yval)
        {
            ammo = 4;
            _ammoType = new ATLaser
            {
                range = 170f,
                accuracy = 0.8f,
                penetration = -1f
            };
            _type = "gun";
            graphic = new Sprite("netGun");
            center = new Vec2(16f, 16f);
            collisionOffset = new Vec2(-8f, -4f);
            collisionSize = new Vec2(16f, 9f);
            _barrelOffsetTL = new Vec2(27f, 14f);
            _fireSound = "smg";
            _fullAuto = true;
            _fireWait = 1f;
            _kickForce = 3f;
            _fireRumble = RumbleIntensity.Kick;
            _netGunGuage = new SpriteMap("netGunGuage", 8, 8);
            _barrelSteam = new SpriteMap("steamPuff", 16, 16)
            {
                center = new Vec2(0f, 14f)
            };
            _barrelSteam.AddAnimation("puff", 0.4f, false, 0, 1, 2, 3, 4, 5, 6, 7);
            _barrelSteam.SetAnimation("puff");
            _barrelSteam.speed = 0f;
            _bio = "C02 powered, shoots nets, traps ducks. Is that stubborn duck not moving? Why not trap it, and put it where it belongs.";
            _editorName = "Net Gun";
            editorTooltip = "Fires entangling nets that hold Ducks in place *evil moustache twirl*";
            isFatal = false;
        }
        public override Holdable BecomeTapedMonster(TapedGun pTaped)
        {
            if (Editor.clientonlycontent)
            {
                return pTaped.gun1 is NetGun && pTaped.gun2 is CampingRifle ? new CampNetgun(x, y) : null;
            }
            return base.BecomeTapedMonster(pTaped);
        }
        public override void Initialize() => base.Initialize();

        public override void Update()
        {
            _netGunGuage.frame = 4 - Math.Min(ammo + 1, 4);
            if (_barrelSteam.speed > 0 && _barrelSteam.finished)
                _barrelSteam.speed = 0f;
            base.Update();
        }

        public override void Draw()
        {
            base.Draw();
            if (_barrelSteam.speed > 0)
            {
                _barrelSteam.alpha = 0.6f;
                Draw(_barrelSteam, new Vec2(9f, 1f));
            }
            Draw(_netGunGuage, new Vec2(-4f, -4f));
        }

        public override void OnPressAction()
        {
            if (ammo > 0)
            {
                --ammo;
                if (duck != null)
                    RumbleManager.AddRumbleEvent(duck.profile, new RumbleEvent(_fireRumble, RumbleDuration.Pulse, RumbleFalloff.None));
                SFX.Play("netGunFire");
                _barrelSteam.speed = 1f;
                _barrelSteam.frame = 0;
                ApplyKick();
                Vec2 vec2 = Offset(barrelOffset);
                if (receivingPress)
                    return;
                Net t = new Net(vec2.x, vec2.y - 2f, duck);
                Level.Add(t);
                Fondle(t);
                if (owner != null)
                    t.responsibleProfile = owner.responsibleProfile;
                t.clip.Add(owner as MaterialThing);
                t.hSpeed = barrelVector.x * 10f;
                t.vSpeed = (float)(barrelVector.y * 7 - 1.5f);
            }
            else
                DoAmmoClick();
        }

        public override void Fire()
        {
        }
    }
}
