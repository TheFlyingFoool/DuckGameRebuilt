using System.Collections.Generic;

namespace DuckGame
{
    [ClientOnly]
    public class CampNetgun : Gun
    {
        public CampNetgun(float xpos, float ypos) : base(xpos, ypos)
        {
            ammo = 4;
            campnet = new Sprite("campnet");
            campnet.CenterOrigin();
            graphic = new Sprite("campnetgun");
            center = new Vec2(12.5f, 8);
            _collisionSize = new Vec2(25, 16);
            _collisionOffset = new Vec2(-12.5f, -8f);
            _barrelOffsetTL = new Vec2(26, 8.5f);
            _kickForce = 4.2f;
        }
        public Sprite campnet;
        public List<Vec2> timld = new List<Vec2>();
        public override void Draw()
        {
            float f = 0;
            if (owner != null) f = owner.hSpeed;
            if (raised)
            {
                if (owner != null) f = owner.vSpeed;
                for (int i = 0; i < ammo; i++)
                {
                    Vec2 v = Offset(new Vec2(5, 4 + i * 5));

                    Graphics.Draw(campnet, v.x, v.y - f * i, depth + (-2 + i));
                }
            }
            else
            {
                for (int i = 0; i < ammo; i++)
                {
                    Vec2 v = Offset(new Vec2(5, 4 + i * 5));

                    Graphics.Draw(campnet, v.x - f * i, v.y, depth + (-2 + i));
                }
            }
            base.Draw();
        }
        public override void Update()
        {
            
            base.Update();
        }
        public override bool CanTapeTo(Thing pThing)
        {
            switch (pThing)
            {
                case NetGun _:
                case CampingRifle _:
                    return false;
                default:
                    return true;
            }
        }
        public override void OnPressAction()
        {
            if (ammo > 0)
            {
                ammo--;
                if (duck != null) RumbleManager.AddRumbleEvent(duck.profile, new RumbleEvent(_fireRumble, RumbleDuration.Pulse, RumbleFalloff.None));
                SFX.Play("netGunFire");
                SFX.Play("netGunFire");
                ApplyKick();
                Vec2 vec2 = Offset(barrelOffset);
                if (receivingPress) return;
                Net t = new Net(vec2.x, vec2.y - 2f, duck);
                Level.Add(t);
                Fondle(t);
                if (owner != null) t.responsibleProfile = owner.responsibleProfile;
                t.clip.Add(owner as MaterialThing);
                t.hSpeed = barrelVector.x * 11f;
                t.vSpeed = (float)(barrelVector.y * 7 - 1.5f);
            }
            else DoAmmoClick();
        }

        public override void Fire()
        {
        }
    }
}
