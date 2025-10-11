using System.Collections.Generic;

namespace DuckGame
{
    [ClientOnly]
    public class CampNetgun : Gun
    {
        public CampNetgun(float xpos, float ypos) : base(xpos, ypos)
        {
            ammo = 5;
            campnet = new Sprite("campnetammo");
            campnet.CenterOrigin();
            graphic = new Sprite("campnetgun");
            center = new Vec2(12.5f, 8);
            _collisionSize = new Vec2(25, 16);
            _collisionOffset = new Vec2(-12.5f, -8f);
            _barrelOffsetTL = new Vec2(26, 8.5f);
            _kickForce = 4.2f;
            _holdOffset = new Vec2(2, 0);
        }
        public Sprite campnet;
        public List<Vec2> timld = new List<Vec2>();
        public override bool CanTapeTo(Thing pThing)
        {
            switch (pThing)
            {
                case NetGun:
                case CampNetgun:
                case CampingRifle:
                    return false;
                default:
                    return true;
            }
        }
        public override void Draw()
        {
            float f = 0;
            if (owner != null) f = owner.hSpeed;
            int ReAmmo = ammo;
            if (infiniteAmmoVal) ReAmmo = 5;
            if (raised)
            {
                if (owner != null) f = owner.vSpeed;
                for (int i = 0; i < ReAmmo; i++)
                {
                    Vec2 v = Offset(new Vec2(5, 4 + i * 3));

                    Graphics.Draw(campnet, v.x, v.y - f * i, depth + (-2 + i));
                }
            }
            else
            {
                for (int i = 0; i < ReAmmo; i++)
                {
                    Vec2 v = Offset(new Vec2(5, 4 + i * 3));

                    Graphics.Draw(campnet, v.x - f * i, v.y, depth + (-2 + i));
                }
            }
            base.Draw();
        }
        public override void Update()
        {
            
            base.Update();
        }
        public override void OnPressAction()
        {
            if (ammo > 0)
            {
                ammo--;
                if (duck != null) RumbleManager.AddRumbleEvent(duck.profile, new RumbleEvent(_fireRumble, RumbleDuration.Pulse, RumbleFalloff.None));
                SFX.Play("netGunFire");
                SFX.Play("campingThwoom");
                ApplyKick();

                Vec2 vec2 = Offset(barrelOffset);
                for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 5; ++index)
                {
                    CampingSmoke campingSmoke = new CampingSmoke((barrelPosition.x - 8f + Rando.Float(8f) + offDir * 8f), barrelPosition.y - 8f + Rando.Float(8f))
                    {
                        depth = (Depth)(float)(0.9f + index * (1f / 1000f))
                    };
                    if (index < 3) campingSmoke.move -= barrelVector * Rando.Float(0.05f);
                    else campingSmoke.fly += barrelVector * (1f + Rando.Float(2.8f));
                    Level.Add(campingSmoke);
                }
                if (receivingPress) return;
                CampingNet t = new CampingNet(vec2.x, vec2.y - 2f);
                Level.Add(t);
                Fondle(t);
                if (owner != null) t.responsibleProfile = owner.responsibleProfile;
                t.clip.Add(owner as MaterialThing);
                t.velocity = barrelVector * 14;
            }
            else DoAmmoClick();
        }

        public override void Fire()
        {
        }
    }
}
