// Decompiled with JetBrains decompiler
// Type: DuckGame.Shotgun
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    [EditorGroup("Guns|Shotguns")]
    public class Shotgun : Gun
    {
        public sbyte _loadProgress = 100;
        public float _loadAnimation = 1f;
        public StateBinding _loadProgressBinding = new StateBinding(nameof(_loadProgress));
        protected SpriteMap _loaderSprite;

        public Shotgun(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 2;
            this._ammoType = new ATShotgun();
            this.wideBarrel = true;
            this._type = "gun";
            this.graphic = new Sprite("shotgun");
            this.center = new Vec2(16f, 16f);
            this.collisionOffset = new Vec2(-8f, -3f);
            this.collisionSize = new Vec2(16f, 8f);
            this._barrelOffsetTL = new Vec2(30f, 14f);
            this._fireSound = "shotgunFire2";
            this._kickForce = 4f;
            this._fireRumble = RumbleIntensity.Light;
            this._numBulletsPerFire = 6;
            this._manualLoad = true;
            this._loaderSprite = new SpriteMap("shotgunLoader", 8, 8)
            {
                center = new Vec2(4f, 4f)
            };
            this.editorTooltip = "It's...a shotgun. I don't really have anything more to say about it.";
        }

        public override void Update()
        {
            base.Update();
            if (_loadAnimation == -1.0)
            {
                SFX.Play("shotgunLoad");
                this._loadAnimation = 0.0f;
            }
            if (_loadAnimation >= 0.0)
            {
                if (_loadAnimation == 0.5 && this.ammo != 0)
                    this.PopShell();
                if (_loadAnimation < 1.0)
                    this._loadAnimation += 0.1f;
                else
                    this._loadAnimation = 1f;
            }
            if (this._loadProgress < 0)
                return;
            if (this._loadProgress == 50)
                this.Reload(false);
            if (this._loadProgress < 100)
                this._loadProgress += 10;
            else
                this._loadProgress = 100;
        }

        public override void OnPressAction()
        {
            if (this.loaded)
            {
                base.OnPressAction();
                this._loadProgress = -1;
                this._loadAnimation = -0.01f;
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
