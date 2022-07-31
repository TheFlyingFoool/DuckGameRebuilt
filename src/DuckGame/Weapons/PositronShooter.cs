// Decompiled with JetBrains decompiler
// Type: DuckGame.PositronShooter
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System.Collections.Generic;

namespace DuckGame
{
    [BaggedProperty("canSpawn", false)]
    public class PositronShooter : Gun
    {
        public StateBinding _burstingBinding = new StateBinding(nameof(_bursting));
        public StateBinding _burstNumBinding = new StateBinding(nameof(_burstNum));
        private List<string> _notes = new List<string>()
    {
      "",
      "CSharp",
      "FSharp",
      "",
      "FSharp",
      "GSharp",
      "",
      "GSharp",
      "ASharp",
      "CSharp2",
      "ASharp",
      "FSharp",
      "",
      "CSharp",
      "FSharp",
      "",
      "FSharp",
      "GSharp",
      "",
      "GSharp",
      "ASharp",
      "",
      "",
      "FSharp",
      "",
      "CSharp",
      "FSharp",
      "",
      "FSharp",
      "GSharp",
      "",
      "GSharp",
      "ASharp",
      "CSharp2",
      "ASharp",
      "FSharp",
      "",
      "",
      "DSharp",
      "",
      "",
      "GSharp",
      "",
      "B",
      "ASharp",
      "",
      "",
      "FSharp"
    };
        public float _burstWait;
        public bool _bursting;
        public int _burstNum;
        private Sprite _positronWinder;
        //private bool _manualWind;
        //private float _prevStick;
        private int _prevInc;
        private bool _winding;
        private float _windVelocity;
        private float _wind;
        private int _noteIndex;
        public static bool inFire;

        public PositronShooter(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 999999;
            this._ammoType = new ATLaserOrange();
            this._ammoType.affectedByGravity = true;
            this._type = "gun";
            this.graphic = new Sprite("positronShooter");
            this.center = new Vec2(10f, 4f);
            this.collisionOffset = new Vec2(-8f, -3f);
            this.collisionSize = new Vec2(16f, 7f);
            this._positronWinder = new Sprite("positronWinder")
            {
                center = new Vec2(1.5f, 6.5f)
            };
            this._barrelOffsetTL = new Vec2(27f, 4f);
            this._fireSound = "laserRifle";
            this._fullAuto = true;
            this._fireWait = 0f;
            this._kickForce = 1f;
            this._fireRumble = RumbleIntensity.Kick;
            this._holdOffset = new Vec2(-4f, -2f);
            this._flare = new SpriteMap("laserFlareOrange", 16, 16)
            {
                center = new Vec2(0f, 8f)
            };
            this.editorTooltip = "A futuristic weapon from the WORLD OF TOMORROW!";
        }

        public override void Update()
        {
            if (this._bursting)
            {
                this._burstWait = Maths.CountDown(this._burstWait, 0.16f);
                if (_burstWait <= 0f)
                {
                    this._burstWait = 1f;
                    if (this.isServerForObject)
                    {
                        PositronShooter.inFire = true;
                        this.Fire();
                        PositronShooter.inFire = false;
                        if (Network.isActive)
                            Send.Message(new NMFireGun(this, this.firedBullets, this.bulletFireIndex, false, this.duck != null ? this.duck.netProfileIndex : (byte)4, true), NetMessagePriority.Urgent);
                        this.firedBullets.Clear();
                    }
                    this._wait = 0f;
                    ++this._burstNum;
                }
                if (this._burstNum == 3)
                {
                    this._burstNum = 0;
                    this._burstWait = 0f;
                    this._bursting = false;
                    this._wait = this._fireWait;
                }
            }
            if (_windVelocity > 0.3f)
                this._windVelocity = 0.3f;
            this._windVelocity = Lerp.Float(this._windVelocity, 0f, 0.0035f);
            if (this._noteIndex <= this._notes.Count)
            {
                this._wind += this._windVelocity;
                if ((int)this._wind > this._prevInc)
                {
                    if (this._noteIndex < this._notes.Count)
                    {
                        if (this._notes[this._noteIndex] != "")
                            SFX.Play("musicBox" + this._notes[this._noteIndex]);
                        ++this._noteIndex;
                    }
                    else
                        this._windVelocity = 0f;
                    ++this._prevInc;
                }
            }
            else
                this._noteIndex = this._notes.Count + 1;
            this._winding = this.duck != null && this.duck.inputProfile.Down("UP");
            base.Update();
        }

        public override void CheckIfHoldObstructed()
        {
            if (this.duck != null && this.duck.inputProfile.Down("UP"))
                this.duck.holdObstructed = true;
            else
                base.CheckIfHoldObstructed();
        }

        public override void Draw()
        {
            Vec2 vec2 = this.Offset(new Vec2(0.5f, 0.5f));
            this._positronWinder.angle = this._wind * offDir;
            Graphics.Draw(this._positronWinder, vec2.x, vec2.y, this.depth + 10);
            base.Draw();
        }

        public override void OnPressAction()
        {
            if (this._winding && this._noteIndex != this._notes.Count)
            {
                //this._manualWind = false;
                this._windVelocity += 0.05f;
            }
            else
            {
                float num = _noteIndex / (float)this._notes.Count;
                this._ammoType.range = 1000f;
                this._ammoType.bulletSpeed = (1f + num * 20f);
                this._ammoType.affectedByGravity = num < 0.6f;
                this._ammoType.accuracy = num;
                this._ammoType.bulletThickness = (0.2f + num * 0.3f);
                this._ammoType.penetration = 0.5f;
                this._fireSound = "awfulLaser";
                this._ammoType.bulletSpeed -= 0.5f;
                if (num > 0.1f)
                {
                    this._ammoType.bulletSpeed += 0.5f;
                    this._ammoType.penetration = 1f;
                    this._fireSound = "phaserSmall";
                }
                if (num > 0.3f)
                {
                    this._ammoType.penetration = 2f;
                    this._fireSound = "phaserMedium";
                }
                if (num > 0.75f)
                {
                    this._ammoType.penetration = 4f;
                    this._fireSound = "laserRifle";
                }
                this._ammoType.bulletLength = 100f;
                if (this._noteIndex == this._notes.Count)
                {
                    this._ammoType.bulletSpeed = 32f;
                    this._ammoType.bulletThickness = 4.4f;
                    this._ammoType.penetration = 100f;
                    this._ammoType.bulletLength = 250f;
                    if (this.duck != null)
                    {
                        this.duck.vSpeed -= 3f;
                        this.duck.hSpeed = duck.offDir * -6;
                        if (this._winding)
                        {
                            this.duck.Swear();
                            this.duck.GoRagdoll();
                            this.hSpeed += offDir * 3f;
                            this.vSpeed -= 3f;
                            this._winding = false;
                            this._windVelocity = 0f;
                        }
                        else
                        {
                            this.duck.sliding = true;
                            this.duck.crippleTimer = 1f;
                        }
                    }
                    this._noteIndex = 0;
                    this._fireSound = "laserBlast";
                }
                if (this._noteIndex > 1)
                    --this._noteIndex;
                this.Fire();
            }
        }

        public override void OnHoldAction()
        {
        }
    }
}
