// Decompiled with JetBrains decompiler
// Type: DuckGame.CampingRifle
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            this.ammo = 4;
            this._ammoType = new ATCampingBall();
            this._type = "gun";
            this._sprite = new SpriteMap("camping", 23, 15)
            {
                speed = 0f
            };
            this.graphic = _sprite;
            this.center = new Vec2(11f, 7f);
            this.collisionOffset = new Vec2(-10f, -5f);
            this.collisionSize = new Vec2(20f, 12f);
            this._barrelOffsetTL = new Vec2(22f, 6f);
            this._fireSound = "shotgunFire2";
            this._kickForce = 4f;
            this._fireRumble = RumbleIntensity.Light;
            this._numBulletsPerFire = 6;
            this._manualLoad = true;
            this.flammable = 1f;
            this._loaderSprite = new SpriteMap("camping_loader", 6, 4)
            {
                center = new Vec2(3f, 2f)
            };
            this._holdOffset = new Vec2(0f, -2f);
            this._editorName = "Camping Gun";
            this.editorTooltip = "Designed to get campers into bed quickly.";
            this.loaded = false;
            this._loadProgress = -1;
            this._loadAnimation = 0f;
            this.isFatal = false;
            this._clickSound = "campingEmpty";
            this.physicsMaterial = PhysicsMaterial.Plastic;
        }

        public override void Update()
        {
            if (!this.burntOut && burnt >= 1.0)
            {
                this._sprite = new SpriteMap("campingMelted", 23, 15);
                for (int index = 0; index < 4; ++index)
                    Level.Add(SmallSmoke.New(Rando.Float(-4f, 4f), Rando.Float(-4f, 4f)));
                this._onFire = false;
                this.flammable = 0f;
                this.ammo = 0;
                this.graphic = _sprite;
                this.burntOut = true;
            }
            base.Update();
            if (_loadAnimation == -1.0)
            {
                SFX.Play("click");
                this._loadAnimation = 0f;
            }
            if (_loadAnimation >= 0.0)
            {
                if (this._loadProgress < 0)
                {
                    if (_loadAnimation < 1.0)
                        this._loadAnimation += 0.1f;
                    else
                        this._loadAnimation = 1f;
                }
                else if (_loadAnimation < 0.5)
                    this._loadAnimation += 0.2f;
                else
                    this._loadAnimation = 0.5f;
            }
            if (this._loadProgress >= 0)
            {
                if (this._loadProgress == 50 && this.isServerForObject)
                {
                    this.Reload(false);
                    this.readyToFire = true;
                }
                if (this._loadProgress < 100)
                    this._loadProgress += 10;
                else
                    this._loadProgress = 100;
            }
            if (this.burntOut)
                return;
            if (this.ammo == 4 || (bool)this.infinite)
                this._sprite.frame = 0;
            else if (this.ammo == 3)
                this._sprite.frame = 1;
            else if (this.ammo == 2)
                this._sprite.frame = 2;
            else
                this._sprite.frame = 3;
        }

        public override void OnPressAction()
        {
            if (this.readyToFire)
            {
                if (this.ammo <= 0 || this.burntOut)
                {
                    this.DoAmmoClick();
                }
                else
                {
                    if (this.duck != null)
                        RumbleManager.AddRumbleEvent(this.duck.profile, new RumbleEvent(this._fireRumble, RumbleDuration.Pulse, RumbleFalloff.None));
                    SFX.Play("campingThwoom");
                    this.ApplyKick();
                    Vec2 vec2 = this.Offset(this.barrelOffset);
                    for (int index = 0; index < 6; ++index)
                    {
                        CampingSmoke campingSmoke = new CampingSmoke((barrelPosition.x - 8f + Rando.Float(8f) + offDir * 8f), this.barrelPosition.y - 8f + Rando.Float(8f))
                        {
                            depth = (Depth)(float)(0.9f + index * (1f / 1000f))
                        };
                        if (index < 3)
                            campingSmoke.move -= this.barrelVector * Rando.Float(0.05f);
                        else
                            campingSmoke.fly += this.barrelVector * (1f + Rando.Float(2.8f));
                        Level.Add(campingSmoke);
                    }
                    if (!this.receivingPress)
                    {
                        CampingBall t = new CampingBall(vec2.x, vec2.y - 2f, this.duck);
                        Level.Add(t);
                        this.Fondle(t);
                        if (this.onFire)
                            t.LightOnFire();
                        if (this.owner != null)
                            t.responsibleProfile = this.owner.responsibleProfile;
                        t.clip.Add(this.owner as MaterialThing);
                        t.hSpeed = this.barrelVector.x * 10f;
                        t.vSpeed = (float)(barrelVector.y * 7.0 - 0.75);
                    }
                }
                this._loadProgress = -1;
                this.readyToFire = false;
                if (this.ammo != 1)
                    return;
                this.ammo = 0;
            }
            else
            {
                if (this._loadProgress != -1)
                    return;
                this._loadProgress = 0;
                this._loadAnimation = -1f;
            }
        }

        public override void Draw()
        {
            base.Draw();
            Vec2 vec2 = new Vec2(13f, -2f);
            float num = (float)Math.Sin(_loadAnimation * 3.14000010490417) * 3f;
            this.Draw(_loaderSprite, new Vec2(vec2.x - 8f - num, vec2.y + 4f));
        }
    }
}
