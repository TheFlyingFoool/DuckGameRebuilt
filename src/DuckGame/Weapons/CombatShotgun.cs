// Decompiled with JetBrains decompiler
// Type: DuckGame.CombatShotgun
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    [EditorGroup("Guns|Shotguns")]
    public class CombatShotgun : Gun
    {
        public StateBinding _readyToShootBinding = new StateBinding(nameof(_readyToShoot));
        private float _loadProgress = 1f;
        public float _loadWait;
        public bool _readyToShoot = true;
        private SpriteMap _loaderSprite;
        private SpriteMap _ammoSprite;
        private int _ammoMax = 6;

        public CombatShotgun(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = this._ammoMax;
            this._ammoType = (AmmoType)new ATShotgun();
            this._ammoType.range = 140f;
            this.wideBarrel = true;
            this._type = "gun";
            this.graphic = new Sprite("combatShotgun");
            this.center = new Vec2(16f, 16f);
            this.collisionOffset = new Vec2(-12f, -3f);
            this.collisionSize = new Vec2(24f, 9f);
            this._barrelOffsetTL = new Vec2(29f, 15f);
            this._fireSound = "shotgunFire2";
            this._kickForce = 5f;
            this._fireRumble = RumbleIntensity.Kick;
            this._numBulletsPerFire = 7;
            this._manualLoad = true;
            this._loaderSprite = new SpriteMap("combatShotgunLoader", 16, 16);
            this._loaderSprite.center = new Vec2(8f, 8f);
            this._ammoSprite = new SpriteMap("combatShotgunAmmo", 16, 16);
            this._ammoSprite.center = new Vec2(8f, 8f);
            this.handOffset = new Vec2(0.0f, 1f);
            this._holdOffset = new Vec2(4f, 0.0f);
            this.editorTooltip = "So many shells, what convenience!";
        }

        public override void Update()
        {
            this._ammoSprite.frame = this._ammoMax - this.ammo;
            base.Update();
            if (this._readyToShoot)
            {
                this._loadProgress = 1f;
                this._loadWait = 0.0f;
            }
            if ((double)this._loadWait > 0.0)
                return;
            if ((double)this._loadProgress == 0.0)
                SFX.Play("shotgunLoad");
            if ((double)this._loadProgress == 0.5)
                this.Reload();
            this._loadWait = 0.0f;
            if ((double)this._loadProgress < 1.0)
            {
                this._loadProgress += 0.1f;
            }
            else
            {
                this._loadProgress = 1f;
                this._readyToShoot = true;
                this._readyToShoot = false;
            }
        }

        public override void OnPressAction()
        {
            if ((double)this._loadProgress >= 1.0)
            {
                base.OnPressAction();
                this._loadProgress = 0.0f;
                this._loadWait = 1f;
            }
            else
            {
                if ((double)this._loadWait != 1.0)
                    return;
                this._loadWait = 0.0f;
            }
        }

        public override void Draw()
        {
            base.Draw();
            Vec2 vec2 = new Vec2(13f, -1f);
            float num = (float)Math.Sin((double)this._loadProgress * 3.14000010490417) * 3f;
            this.Draw((Sprite)this._loaderSprite, new Vec2(vec2.x - 12f - num, vec2.y + 4f));
            this.Draw((Sprite)this._ammoSprite, new Vec2(-3f, -2f), 2);
        }
    }
}
