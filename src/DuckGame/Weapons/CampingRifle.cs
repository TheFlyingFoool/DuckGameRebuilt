// Decompiled with JetBrains decompiler
// Type: DuckGame.CampingRifle
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    [EditorGroup("Guns|Misc")]
    public class CampingRifle : Gun
    {
        public StateBinding _loadProgressBinding = new StateBinding(nameof(_loadProgress));
        public StateBinding _readyToFireBinding = new StateBinding(nameof(readyToFire));
        public sbyte _loadProgress = 100;
        public float _loadAnimation = 1f;
        public bool readyToFire;
        protected SpriteMap _loaderSprite;
        private SpriteMap _sprite;
        public bool burntOut;

        public CampingRifle(float xval, float yval)
          : base(xval, yval)
        {
            ammo = 4;
            _ammoType = new ATCampingBall();
            _type = "gun";
            _sprite = new SpriteMap("camping", 23, 15)
            {
                speed = 0f
            };
            graphic = _sprite;
            center = new Vec2(11f, 7f);
            collisionOffset = new Vec2(-10f, -5f);
            collisionSize = new Vec2(20f, 12f);
            _barrelOffsetTL = new Vec2(22f, 6f);
            _fireSound = "shotgunFire2";
            _kickForce = 4f;
            _fireRumble = RumbleIntensity.Light;
            _numBulletsPerFire = 6;
            _manualLoad = true;
            flammable = 1f;
            _loaderSprite = new SpriteMap("camping_loader", 6, 4)
            {
                center = new Vec2(3f, 2f)
            };
            _holdOffset = new Vec2(0f, -2f);
            _editorName = "Camping Gun";
            editorTooltip = "Designed to get campers into bed quickly.";
            loaded = false;
            _loadProgress = -1;
            _loadAnimation = 0f;
            isFatal = false;
            _clickSound = "campingEmpty";
            physicsMaterial = PhysicsMaterial.Plastic;
        }

        public override void Update()
        {
            if (!burntOut && burnt >= 1.0)
            {
                _sprite = new SpriteMap("campingMelted", 23, 15);
                for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 4; ++index)
                    Level.Add(SmallSmoke.New(Rando.Float(-4f, 4f), Rando.Float(-4f, 4f)));
                _onFire = false;
                flammable = 0f;
                ammo = 0;
                graphic = _sprite;
                burntOut = true;
            }
            base.Update();
            if (_loadAnimation == -1.0)
            {
                SFX.Play("click");
                _loadAnimation = 0f;
            }
            if (_loadAnimation >= 0.0)
            {
                if (_loadProgress < 0)
                {
                    if (_loadAnimation < 1.0)
                        _loadAnimation += 0.1f;
                    else
                        _loadAnimation = 1f;
                }
                else if (_loadAnimation < 0.5)
                    _loadAnimation += 0.2f;
                else
                    _loadAnimation = 0.5f;
            }
            if (_loadProgress >= 0)
            {
                if (_loadProgress == 50 && isServerForObject)
                {
                    Reload(false);
                    readyToFire = true;
                }
                if (_loadProgress < 100)
                    _loadProgress += 10;
                else
                    _loadProgress = 100;
            }
            if (burntOut)
                return;
            if (ammo == 4 || (bool)infinite)
                _sprite.frame = 0;
            else if (ammo == 3)
                _sprite.frame = 1;
            else if (ammo == 2)
                _sprite.frame = 2;
            else
                _sprite.frame = 3;
        }

        public override void OnPressAction()
        {
            if (readyToFire)
            {
                if (ammo <= 0 || burntOut)
                {
                    DoAmmoClick();
                }
                else
                {
                    if (duck != null)
                        RumbleManager.AddRumbleEvent(duck.profile, new RumbleEvent(_fireRumble, RumbleDuration.Pulse, RumbleFalloff.None));
                    SFX.Play("campingThwoom");
                    ApplyKick();
                    Vec2 vec2 = Offset(barrelOffset);
                    for (int index = 0; index < DGRSettings.ActualParticleMultiplier * 6; ++index)
                    {
                        CampingSmoke campingSmoke = new CampingSmoke((barrelPosition.x - 8f + Rando.Float(8f) + offDir * 8f), barrelPosition.y - 8f + Rando.Float(8f))
                        {
                            depth = (Depth)(float)(0.9f + index * (1f / 1000f))
                        };
                        if (index < 3)
                            campingSmoke.move -= barrelVector * Rando.Float(0.05f);
                        else
                            campingSmoke.fly += barrelVector * (1f + Rando.Float(2.8f));
                        Level.Add(campingSmoke);
                    }
                    if (!receivingPress)
                    {
                        CampingBall t = new CampingBall(vec2.x, vec2.y - 2f, duck);
                        Level.Add(t);
                        Fondle(t);
                        if (onFire)
                            t.LightOnFire();
                        if (owner != null)
                            t.responsibleProfile = owner.responsibleProfile;
                        t.clip.Add(owner as MaterialThing);
                        t.hSpeed = barrelVector.x * 10f;
                        t.vSpeed = (float)(barrelVector.y * 7.0 - 0.75);
                    }
                }
                _loadProgress = -1;
                readyToFire = false;
                if (ammo != 1)
                    return;
                ammo = 0;
            }
            else
            {
                if (_loadProgress != -1)
                    return;
                _loadProgress = 0;
                _loadAnimation = -1f;
            }
        }

        public override void Draw()
        {
            base.Draw();
            Vec2 vec2 = new Vec2(13f, -2f);
            float num = (float)Math.Sin(_loadAnimation * 3.14f) * 3f;
            Draw(_loaderSprite, new Vec2(vec2.x - 8f - num, vec2.y + 4f));
        }
    }
}
