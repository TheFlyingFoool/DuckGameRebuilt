// Decompiled with JetBrains decompiler
// Type: DuckGame.PositronShooter
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
        public float _wind;
        private int _noteIndex;
        public static bool inFire;

        public PositronShooter(float xval, float yval)
          : base(xval, yval)
        {
            ammo = 999999;
            _ammoType = new ATLaserOrange
            {
                affectedByGravity = true
            };
            _type = "gun";
            graphic = new Sprite("positronShooter");
            center = new Vec2(10f, 4f);
            collisionOffset = new Vec2(-8f, -3f);
            collisionSize = new Vec2(16f, 7f);
            _positronWinder = new Sprite("positronWinder")
            {
                center = new Vec2(1.5f, 6.5f)
            };
            _barrelOffsetTL = new Vec2(27f, 4f);
            _fireSound = "laserRifle";
            _fullAuto = true;
            _fireWait = 0f;
            _kickForce = 1f;
            _fireRumble = RumbleIntensity.Kick;
            _holdOffset = new Vec2(-4f, -2f);
            _flare = new SpriteMap("laserFlareOrange", 16, 16)
            {
                center = new Vec2(0f, 8f)
            };
            editorTooltip = "A futuristic weapon from the WORLD OF TOMORROW!";
        }

        public override void Update()
        {
            if (_bursting)
            {
                _burstWait = Maths.CountDown(_burstWait, 0.16f);
                if (_burstWait <= 0f)
                {
                    _burstWait = 1f;
                    if (isServerForObject)
                    {
                        inFire = true;
                        Fire();
                        inFire = false;
                        if (Network.isActive)
                            Send.Message(new NMFireGun(this, firedBullets, bulletFireIndex, false, duck != null ? duck.netProfileIndex : (byte)4, true), NetMessagePriority.Urgent);
                        firedBullets.Clear();
                    }
                    _wait = 0f;
                    ++_burstNum;
                }
                if (_burstNum == 3)
                {
                    _burstNum = 0;
                    _burstWait = 0f;
                    _bursting = false;
                    _wait = _fireWait;
                }
            }
            if (_windVelocity > 0.3f)
                _windVelocity = 0.3f;
            _windVelocity = Lerp.Float(_windVelocity, 0f, 0.0035f);
            if (_noteIndex <= _notes.Count)
            {
                _wind += _windVelocity;
                if ((int)_wind > _prevInc)
                {
                    if (_noteIndex < _notes.Count)
                    {
                        if (_notes[_noteIndex] != "" && !Recorderator.Playing)
                        {
                            SFX.Play("musicBox" + _notes[_noteIndex]);
                        }
                        ++_noteIndex;
                    }
                    else
                        _windVelocity = 0f;
                    ++_prevInc;
                }
            }
            else
                _noteIndex = _notes.Count + 1;
            _winding = duck != null && duck.inputProfile.Down(Triggers.Up);
            base.Update();
        }

        public override void CheckIfHoldObstructed()
        {
            if (duck != null && duck.inputProfile.Down(Triggers.Up))
                duck.holdObstructed = true;
            else
                base.CheckIfHoldObstructed();
        }

        public override void Draw()
        {
            Vec2 vec2 = Offset(new Vec2(0.5f, 0.5f));
            _positronWinder.angle = _wind * offDir;
            Graphics.Draw(_positronWinder, vec2.x, vec2.y, depth + 10);
            base.Draw();
        }

        public override void OnPressAction()
        {
            if (_winding && _noteIndex != _notes.Count)
            {
                //this._manualWind = false;
                _windVelocity += 0.05f;
            }
            else
            {
                float num = _noteIndex / (float)_notes.Count;
                _ammoType.range = 1000f;
                _ammoType.bulletSpeed = (1f + num * 20f);
                _ammoType.affectedByGravity = num < 0.6f;
                _ammoType.accuracy = num;
                _ammoType.bulletThickness = (0.2f + num * 0.3f);
                _ammoType.penetration = 0.5f;
                _fireSound = "awfulLaser";
                _ammoType.bulletSpeed -= 0.5f;
                if (num > 0.1f)
                {
                    _ammoType.bulletSpeed += 0.5f;
                    _ammoType.penetration = 1f;
                    _fireSound = "phaserSmall";
                }
                if (num > 0.3f)
                {
                    _ammoType.penetration = 2f;
                    _fireSound = "phaserMedium";
                }
                if (num > 0.75f)
                {
                    _ammoType.penetration = 4f;
                    _fireSound = "laserRifle";
                }
                _ammoType.bulletLength = 100f;
                if (_noteIndex == _notes.Count)
                {
                    _ammoType.bulletSpeed = 32f;
                    _ammoType.bulletThickness = 4.4f;
                    _ammoType.penetration = 100f;
                    _ammoType.bulletLength = 250f;
                    if (duck != null)
                    {
                        duck.vSpeed -= 3f;
                        duck.hSpeed = duck.offDir * -6;
                        if (_winding)
                        {
                            duck.Swear();
                            duck.GoRagdoll();
                            hSpeed += offDir * 3f;
                            vSpeed -= 3f;
                            _winding = false;
                            _windVelocity = 0f;
                        }
                        else
                        {
                            duck.sliding = true;
                            duck.crippleTimer = 1f;
                        }
                    }
                    _noteIndex = 0;
                    _fireSound = "laserBlast";
                }
                if (_noteIndex > 1)
                    --_noteIndex;
                Fire();
            }
        }

        public override void OnHoldAction()
        {
        }
    }
}
