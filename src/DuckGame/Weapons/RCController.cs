// Decompiled with JetBrains decompiler
// Type: DuckGame.RCController
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Guns|Misc")]
    [BaggedProperty("canSpawn", false)]
    public class RCController : Gun
    {
        public StateBinding _carBinding = new StateBinding(nameof(_car));
        public StateBinding _burningBinding = new StateBinding(nameof(_burning));
        public StateBinding _burnLifeBinding = new StateBinding(nameof(_burnLife));
        private SpriteMap _sprite;
        public bool _burning;
        public float _burnLife = 1f;
        public float _burnWait;
        public RCCar _car;
        private bool _pressed;
        private int _inc;
        private Thing lockedOwner;

        public RCController(float xval, float yval, RCCar car)
          : base(xval, yval)
        {
            this.ammo = 99;
            this._ammoType = new ATLaser();
            this._ammoType.range = 170f;
            this._ammoType.accuracy = 0.8f;
            this._type = "gun";
            this._sprite = new SpriteMap("rcController", 32, 32);
            this.graphic = _sprite;
            this.center = new Vec2(16f, 16f);
            this.collisionOffset = new Vec2(-6f, -4f);
            this.collisionSize = new Vec2(12f, 9f);
            this._barrelOffsetTL = new Vec2(26f, 14f);
            this._fireSound = "smg";
            this._fullAuto = true;
            this._fireWait = 1f;
            this._kickForce = 1f;
            this.flammable = 0.8f;
            this._car = car;
            this.editorTooltip = "Allows you to drive a small car strapped with explosives. Every Duck's dream!";
        }

        public override void Initialize() => base.Initialize();

        public override void Update()
        {
            if (this._car == null && !(Level.current is Editor) && this.isServerForObject)
            {
                this._car = new RCCar(this.x, this.y)
                {
                    _controller = this
                };
                Level.Add(_car);
            }
            ++this._inc;
            if (this._inc > 14 && this._car != null)
            {
                this._inc = 0;
                if (this._car.receivingSignal && !this._car.destroyed)
                    Level.Add(new RCControlBolt(this.x, this.y, this._car));
            }
            if (this.lockedOwner != this.owner)
                this.Release(this.lockedOwner);
            if (this.isServerForObject)
            {
                if (this._burning && _burnLife > 0.0)
                {
                    this._burnWait -= 0.01f;
                    if (_burnWait < 0.0)
                    {
                        Level.Add(SmallFire.New(8f, 0f, 0f, 0f, stick: this, canMultiply: false, firedFrom: this));
                        this._burnWait = 1f;
                    }
                    this._burnLife -= 1f / 500f;
                    if (_burnLife <= 0.0)
                        this._sprite.frame = 1;
                }
                if (this.owner is Duck owner)
                {
                    if (this._pressed && this._car != null)
                    {
                        if (this._car.owner == null)
                            this.Fondle(_car);
                        this._car.moveLeft = owner.inputProfile.Down("LEFT");
                        this._car.moveRight = owner.inputProfile.Down("RIGHT");
                        this._car.jump = owner.inputProfile.Pressed("JUMP");
                        if (owner.inputProfile.Pressed("GRAB"))
                        {
                            this.Fondle(_car);
                            this._car.Destroy();
                        }
                    }
                    else
                        this._car.moveLeft = this._car.moveRight = this._car.jump = false;
                }
                if (this._car != null && this._car.destroyed)
                    this.ammo = 0;
            }
            base.Update();
        }

        protected override bool OnBurn(Vec2 firePosition, Thing litBy)
        {
            this._burning = true;
            return true;
        }

        public override void Draw() => base.Draw();

        public override void OnPressAction()
        {
            this._pressed = true;
            if (this._car == null || this._car.destroyed)
                return;
            if (this.owner is Duck owner)
            {
                owner.immobilized = true;
                owner.remoteControl = true;
            }
            if (this.lockedOwner != null)
                this.Release(this.lockedOwner);
            this.lockedOwner = this.owner;
            this._car.receivingSignal = true;
            if (!(Level.current.camera is FollowCam camera))
                return;
            camera.Add(_car);
        }

        private void Release(Thing pOwner)
        {
            if (!(pOwner is Duck duck))
                duck = this.prevOwner as Duck;
            if (duck != null)
            {
                duck.immobilized = false;
                duck.remoteControl = false;
            }
            this._pressed = false;
            this.lockedOwner = null;
            if (this._car == null)
                return;
            this._car.receivingSignal = false;
            if (!(Level.current.camera is FollowCam camera))
                return;
            camera.Remove(_car);
        }

        public override void OnReleaseAction() => this.Release(this.owner);

        public override void Fire()
        {
        }
    }
}
