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
        private ConstantSound _idle;

        public bool receivingSignal
        {
            get => _receivingSignal;
            set
            {
                if (_receivingSignal != value && !destroyed)
                {
                    if (value)
                        SFX.Play("rcConnect", 0.5f);
                    else
                        SFX.Play("rcDisconnect", 0.5f);
                }
                _receivingSignal = value;
            }
        }

        public RCCar(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _sprite = new SpriteMap("rcBody", 32, 32);
            _sprite.AddAnimation("idle", 1f, true, new int[1]);
            _sprite.AddAnimation("beep", 0.2f, true, 0, 1);
            graphic = _sprite;
            center = new Vec2(16f, 24f);
            collisionOffset = new Vec2(-8f, 0f);
            collisionSize = new Vec2(16f, 11f);
            depth = -0.5f;
            _editorName = "RC Car";
            thickness = 2f;
            weight = 5f;
            flammable = 0.3f;
            _wheel = new Sprite("rcWheel")
            {
                center = new Vec2(4f, 4f)
            };
            weight = 0.5f;
            physicsMaterial = PhysicsMaterial.Metal;
            if (Editor.clientonlycontent) tapedIndexPreference = 0;
        }
        public override void PreUpdateTapedPositioning(TapedGun pTaped)
        {
            if (Editor.clientonlycontent && receivingSignal)
            {
                //DevConsole.Log(velocity.ToString());
                pTaped.velocity = velocity;

                if (Math.Abs(hSpeed) > 0.3f)
                {
                    offDir = hSpeed > 0 ? (sbyte)1 : (sbyte)-1;
                    pTaped.offDir = offDir;
                    //offDir *= -1;
                }
                pTaped.gun2.enablePhysics = false;//jank -NiK0
                enablePhysics = true;
                pTaped.angle = 0;
            }
            base.PreUpdateTapedPositioning(pTaped);
        }
        public override void UpdateTapedPositioning(TapedGun pTaped)
        {
            base.UpdateTapedPositioning(pTaped);
            if (Editor.clientonlycontent) angle = 0;
        }
        public override void Initialize()
        {
            _idle = new ConstantSound("rcDrive");
        }

        public override void Terminate()
        {
            _idle.Kill();
            _idle.lerpVolume = 0f;
        }

        protected override bool OnDestroy(DestroyType type = null)
        {
            if (type == null && isServerForObject && Editor.clientonlycontent && tapedIsGun1)
            {
                if (tapedCompatriot is Gun g && g.ammo > 0 && g is not MindControlRay)
                {
                    if (g is HugeLaser hl)
                    {
                        g.OnPressAction();
                        hl.doBlast = true;
                    }
                    else g.PressAction();
                    return false;
                }
            }
            RumbleManager.AddRumbleEvent(position, new RumbleEvent(RumbleIntensity.Heavy, RumbleDuration.Short, RumbleFalloff.Medium));
            if (!isServerForObject) return false;
            new ATRCShrapnel().MakeNetEffect(position, false);
            List<Bullet> varBullets = new List<Bullet>();
            for (int index = 0; index < 20; ++index)
            {
                float num = (float)(index * 18 - 5) + Rando.Float(10f);
                ATRCShrapnel type1 = new ATRCShrapnel
                {
                    range = 55f + Rando.Float(14f)
                };
                Bullet bullet = new Bullet(x + (float)(Math.Cos(Maths.DegToRad(num)) * 6), y - (float)(Math.Sin(Maths.DegToRad(num)) * 6), type1, num)
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
            if (Level.current.camera is FollowCam camera) camera.Remove(this);
            if (Recorder.currentRecording != null) Recorder.currentRecording.LogBonus();
            return true;
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if (bullet.isLocal && owner == null) Fondle(this, DuckNetwork.localConnection);
            if (bullet.isLocal) Destroy(new DTShot(bullet));
            return false;
        }
        public override Holdable BecomeTapedMonster(TapedGun pTaped)
        {
            if (Editor.clientonlycontent)
            {
                if (pTaped.gun1 is RCCar rc1 && pTaped.gun2 is RCCar rc2)
                {
                    rc1._destroyed = true;
                    rc2._destroyed = true;
                    return new SuperRCCar(x, y);
                }
            }
            return base.BecomeTapedMonster(pTaped);
        }
        public override void Update()
        {
            if (_controller == null && !(Level.current is Editor) && isServerForObject)
            {
                _controller = new RCController(x, y, this);
                Level.Add(_controller);
            }
            _wave.Update();
            base.Update();
            _sprite.currentAnimation = _receivingSignal ? "beep" : "idle";
            _idle.lerpVolume = Math.Min(_idleSpeed * 10f, 0.7f);
            if (_destroyed)
            {
                _idle.lerpVolume = 0f;
                _idle.lerpSpeed = 1f;
            }
            _idle.pitch = (float)(0.5 + _idleSpeed * 0.5);
            if (moveLeft)
            {
                if (isServerForObject)
                {
                    if (hSpeed > -_maxSpeed)
                        hSpeed -= 0.4f;
                    else
                        hSpeed = -_maxSpeed;
                    offDir = -1;
                }
                _idleSpeed += 0.03f;
                ++_inc;
            }
            if (moveRight)
            {
                if (isServerForObject)
                {
                    if (hSpeed < _maxSpeed)
                        hSpeed += 0.4f;
                    else
                        hSpeed = _maxSpeed;
                    offDir = 1;
                }
                _idleSpeed += 0.03f;
                ++_inc;
            }
            if (_idleSpeed > 0.1f)
            {
                _inc = 0;
                if (DGRSettings.ActualParticleMultiplier >= 1) for (int i = 0; i < DGRSettings.ActualParticleMultiplier; i++) Level.Add(SmallSmoke.New(x - offDir * 10, y));
                else if (Rando.Float(1) < DGRSettings.ActualParticleMultiplier) Level.Add(SmallSmoke.New(x - offDir * 10, y));
            }
            if (!moveLeft && !moveRight) _idleSpeed -= 0.03f;
            if (_idleSpeed > 1) _idleSpeed = 1f;
            if (_idleSpeed < 0) _idleSpeed = 0f;
            if (jump && grounded)
                vSpeed -= 4.8f;
            _tilt = MathHelper.Lerp(_tilt, -hSpeed, 0.4f);
            _waveMult = MathHelper.Lerp(_waveMult, -hSpeed, 0.1f);
            angleDegrees = (float)(_tilt * 2 + _wave.value * (_waveMult * (_maxSpeed - Math.Abs(hSpeed))));

            if (!isServerForObject || !isOffBottomOfLevel || destroyed) return;
            Destroy(new DTFall());
        }

        public override void Draw()
        {
            if (Editor.clientonlycontent && tapedIsGun1) offDir *= -1;
            if (owner == null) _sprite.flipH = offDir < 0;
            base.Draw();
            _wheel.scale = scale;
            Graphics.Draw(_wheel, x - 7f * xscale, y + 9f * yscale);
            Graphics.Draw(_wheel, x + 7f * xscale, y + 9f * yscale);
            if (Editor.clientonlycontent && tapedIsGun1) offDir *= -1;
        }
    }
}
