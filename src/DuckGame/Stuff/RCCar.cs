// Decompiled with JetBrains decompiler
// Type: DuckGame.RCCar
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;

namespace DuckGame
{
    [BaggedProperty("canSpawn", false)]
    public class RCCar : Holdable, IPlatform
    {
        public StateBinding _controllerBinding = new StateBinding(nameof(_controller));
        public StateBinding _signalBinding = new StateBinding(nameof(receivingSignal));
        public StateBinding _idleSpeedBinding = new CompressedFloatBinding(nameof(_idleSpeed), bits: 4);
        private SpriteMap _sprite;
        private float _tilt;
        private float _maxSpeed = 6f;
        private SinWaveManualUpdate _wave = new SinWaveManualUpdate(0.1f);
        private float _waveMult;
        private Sprite _wheel;
        public bool moveLeft;
        public bool moveRight;
        public bool jump;
        private bool _receivingSignal;
        private int _inc;
        public float _idleSpeed;
        public RCController _controller;
        private ConstantSound _idle = new ConstantSound("rcDrive");

        public bool receivingSignal
        {
            get => this._receivingSignal;
            set
            {
                if (this._receivingSignal != value && !this.destroyed)
                {
                    if (value)
                        SFX.Play("rcConnect", 0.5f);
                    else
                        SFX.Play("rcDisconnect", 0.5f);
                }
                this._receivingSignal = value;
            }
        }

        public RCCar(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this._sprite = new SpriteMap("rcBody", 32, 32);
            this._sprite.AddAnimation("idle", 1f, true, new int[1]);
            this._sprite.AddAnimation("beep", 0.2f, true, 0, 1);
            this.graphic = _sprite;
            this.center = new Vec2(16f, 24f);
            this.collisionOffset = new Vec2(-8f, 0.0f);
            this.collisionSize = new Vec2(16f, 11f);
            this.depth = -0.5f;
            this._editorName = "RC Car";
            this.thickness = 2f;
            this.weight = 5f;
            this.flammable = 0.3f;
            this._wheel = new Sprite("rcWheel")
            {
                center = new Vec2(4f, 4f)
            };
            this.weight = 0.5f;
            this.physicsMaterial = PhysicsMaterial.Metal;
        }

        public override void Initialize()
        {
        }

        public override void Terminate()
        {
            this._idle.Kill();
            this._idle.lerpVolume = 0.0f;
        }

        protected override bool OnDestroy(DestroyType type = null)
        {
            RumbleManager.AddRumbleEvent(this.position, new RumbleEvent(RumbleIntensity.Heavy, RumbleDuration.Short, RumbleFalloff.Medium));
            if (!this.isServerForObject)
                return false;
            new ATRCShrapnel().MakeNetEffect(this.position, false);
            List<Bullet> varBullets = new List<Bullet>();
            for (int index = 0; index < 20; ++index)
            {
                float num = (float)(index * 18.0 - 5.0) + Rando.Float(10f);
                ATRCShrapnel type1 = new ATRCShrapnel
                {
                    range = 55f + Rando.Float(14f)
                };
                Bullet bullet = new Bullet(this.x + (float)(Math.Cos((double)Maths.DegToRad(num)) * 6.0), this.y - (float)(Math.Sin((double)Maths.DegToRad(num)) * 6.0), type1, num)
                {
                    firedFrom = this
                };
                varBullets.Add(bullet);
                Level.Add(bullet);
            }
            if (Network.isActive)
            {
                Send.Message(new NMFireGun(null, varBullets, 0, false), NetMessagePriority.ReliableOrdered);
                varBullets.Clear();
            }
            Level.Remove(this);
            if (Level.current.camera is FollowCam camera)
                camera.Remove(this);
            if (Recorder.currentRecording != null)
                Recorder.currentRecording.LogBonus();
            return true;
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if (bullet.isLocal && this.owner == null)
                Thing.Fondle(this, DuckNetwork.localConnection);
            if (bullet.isLocal)
                this.Destroy(new DTShot(bullet));
            return false;
        }

        public override void Update()
        {
            if (this._controller == null && !(Level.current is Editor) && this.isServerForObject)
            {
                this._controller = new RCController(this.x, this.y, this);
                Level.Add(_controller);
            }
            this._wave.Update();
            base.Update();
            this._sprite.currentAnimation = this._receivingSignal ? "beep" : "idle";
            this._idle.lerpVolume = Math.Min(this._idleSpeed * 10f, 0.7f);
            if (this._destroyed)
            {
                this._idle.lerpVolume = 0.0f;
                this._idle.lerpSpeed = 1f;
            }
            this._idle.pitch = (float)(0.5 + _idleSpeed * 0.5);
            if (this.moveLeft)
            {
                if (this.isServerForObject)
                {
                    if ((double)this.hSpeed > -(double)this._maxSpeed)
                        this.hSpeed -= 0.4f;
                    else
                        this.hSpeed = -this._maxSpeed;
                    this.offDir = -1;
                }
                this._idleSpeed += 0.03f;
                ++this._inc;
            }
            if (this.moveRight)
            {
                if (this.isServerForObject)
                {
                    if ((double)this.hSpeed < _maxSpeed)
                        this.hSpeed += 0.4f;
                    else
                        this.hSpeed = this._maxSpeed;
                    this.offDir = 1;
                }
                this._idleSpeed += 0.03f;
                ++this._inc;
            }
            if (_idleSpeed > 0.100000001490116)
            {
                this._inc = 0;
                Level.Add(SmallSmoke.New(this.x - offDir * 10, this.y));
            }
            if (!this.moveLeft && !this.moveRight)
                this._idleSpeed -= 0.03f;
            if (_idleSpeed > 1.0)
                this._idleSpeed = 1f;
            if (_idleSpeed < 0.0)
                this._idleSpeed = 0.0f;
            if (this.jump && this.grounded)
                this.vSpeed -= 4.8f;
            this._tilt = MathHelper.Lerp(this._tilt, -this.hSpeed, 0.4f);
            this._waveMult = MathHelper.Lerp(this._waveMult, -this.hSpeed, 0.1f);
            this.angleDegrees = (float)(_tilt * 2.0 + (double)this._wave.value * (_waveMult * (_maxSpeed - (double)Math.Abs(this.hSpeed))));
            if (!this.isServerForObject || !this.isOffBottomOfLevel || this.destroyed)
                return;
            this.Destroy(new DTFall());
        }

        public override void Draw()
        {
            if (this.owner == null)
                this._sprite.flipH = offDir < 0.0;
            base.Draw();
            Graphics.Draw(this._wheel, this.x - 7f, this.y + 9f);
            Graphics.Draw(this._wheel, this.x + 7f, this.y + 9f);
        }
    }
}
