// Decompiled with JetBrains decompiler
// Type: DuckGame.Phaser
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Guns|Lasers")]
    public class Phaser : Gun
    {
        private float _charge;
        private int _chargeLevel;
        private float _chargeFade;
        private SinWave _chargeWaver = (SinWave)0.4f;
        private SpriteMap _phaserCharge;

        public Phaser(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 30;
            this._ammoType = new ATPhaser();
            this._type = "gun";
            this.graphic = new Sprite("phaser");
            this.center = new Vec2(7f, 4f);
            this.collisionOffset = new Vec2(-7f, -4f);
            this.collisionSize = new Vec2(15f, 9f);
            this._barrelOffsetTL = new Vec2(14f, 3f);
            this._fireSound = "laserRifle";
            this._fullAuto = false;
            this._fireWait = 0.0f;
            this._kickForce = 0.5f;
            this._holdOffset = new Vec2(0.0f, 0.0f);
            this._flare = new SpriteMap("laserFlare", 16, 16)
            {
                center = new Vec2(0.0f, 8f)
            };
            this._phaserCharge = new SpriteMap("phaserCharge", 8, 8)
            {
                frame = 1
            };
            this.editorTooltip = "Like a laser, only...phasery? Hold the trigger to charge a more powerful shot.";
        }

        public override void Update()
        {
            if (this.owner == null || this.ammo <= 0)
            {
                this._charge = 0f;
                this._chargeLevel = 0;
            }
            this._chargeFade = Lerp.Float(this._chargeFade, _chargeLevel / 3f, 0.06f);
            base.Update();
        }

        public override void OnPressAction()
        {
        }

        public override void Draw()
        {
            base.Draw();
            if (_chargeFade <= 0.01f)
                return;
            float alpha = this.alpha;
            this.alpha = ((_chargeFade * 0.6f + _chargeFade * this._chargeWaver.normalized * 0.4f) * 0.8f);
            this.Draw(_phaserCharge, new Vec2((3f + _chargeFade * this._chargeWaver * 0.5f), -4f), -1);
            this.alpha = alpha;
        }

        public override void OnHoldAction()
        {
            if (this.ammo <= 0)
                return;
            this._charge += 0.03f;
            if (_charge > 1f)
                this._charge = 1f;
            if (this._chargeLevel == 0)
                this._chargeLevel = 1;
            else if (_charge > 0.4f && this._chargeLevel == 1)
            {
                this._chargeLevel = 2;
                SFX.Play("phaserCharge02", 0.5f);
            }
            else
            {
                if (_charge <= 0.8f || this._chargeLevel != 2)
                    return;
                this._chargeLevel = 3;
                SFX.Play("phaserCharge03", 0.6f);
            }
        }

        public override void OnReleaseAction()
        {
            if (this.ammo <= 0)
                return;
            if (this.owner != null)
            {
                this._ammoType.range = _chargeLevel * 80f;
                this._ammoType.bulletThickness = (float)(0.2f + _charge * 0.4f);
                this._ammoType.penetration = _chargeLevel;
                this._ammoType.accuracy = (float)(0.4f + _charge * 0.5f);
                this._ammoType.bulletSpeed = (float)(8f + _charge * 10f);
                if (this._chargeLevel == 1)
                {
                    if (this.duck != null)
                        RumbleManager.AddRumbleEvent(this.duck.profile, new RumbleEvent(RumbleIntensity.Kick, RumbleDuration.Pulse, RumbleFalloff.None));
                    this._fireSound = "phaserSmall";
                }
                else if (this._chargeLevel == 2)
                {
                    if (this.duck != null)
                        RumbleManager.AddRumbleEvent(this.duck.profile, new RumbleEvent(RumbleIntensity.Light, RumbleDuration.Pulse, RumbleFalloff.None));
                    this._fireSound = "phaserMedium";
                }
                else if (this._chargeLevel == 3)
                {
                    if (this.duck != null)
                        RumbleManager.AddRumbleEvent(this.duck.profile, new RumbleEvent(RumbleIntensity.Light, RumbleDuration.Pulse, RumbleFalloff.Short));
                    this._fireSound = "phaserLarge";
                }
                this.Fire();
                this._charge = 0f;
                this._chargeLevel = 0;
            }
            base.OnReleaseAction();
        }
    }
}
