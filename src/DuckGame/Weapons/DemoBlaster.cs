// Decompiled with JetBrains decompiler
// Type: DuckGame.DemoBlaster
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [BaggedProperty("canSpawn", false)]
    [BaggedProperty("isFatal", false)]
    [BaggedProperty("isInDemo", true)]
    public class DemoBlaster : Gun
    {
        private FluidStream _stream;
        private ConstantSound _sound = new ConstantSound("demoBlaster");
        private int _wait;

        public DemoBlaster(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 9;
            this._ammoType = (AmmoType)new AT9mm();
            this._type = "gun";
            this.graphic = new Sprite("demoBlaster");
            this.center = new Vec2(18f, 8f);
            this.collisionOffset = new Vec2(-16f, -8f);
            this.collisionSize = new Vec2(32f, 15f);
            this._barrelOffsetTL = new Vec2(37f, 7f);
            this._fireSound = "pistolFire";
            this._kickForce = 3f;
            this._fireRumble = RumbleIntensity.Kick;
            this._holdOffset = new Vec2(-1f, -4f);
            this.weight = 8f;
            this.loseAccuracy = 0.1f;
            this.maxAccuracyLost = 0.6f;
            this._bio = "";
            this._editorName = "Demo Blaster";
            this.physicsMaterial = PhysicsMaterial.Metal;
            this._stream = new FluidStream(this.x, this.y, new Vec2(1f, 0.0f), 1f);
            this.isFatal = false;
        }

        public override void Initialize() => Level.Add((Thing)this._stream);

        public override void Terminate() => Level.Remove((Thing)this._stream);

        public override void Update()
        {
            this._sound.lerpVolume = this._triggerHeld ? 1f : 0.0f;
            base.Update();
        }

        public override void OnPressAction()
        {
        }

        public override void OnHoldAction()
        {
            this._wait++;
            if (this._wait == 3)
            {
                this._stream.sprayAngle = base.barrelVector * 2f;
                this._stream.position = base.barrelPosition;
                global::DuckGame.FluidData dat = global::DuckGame.Fluid.Poo;
                dat.amount = 0.01f;
                this._stream.Feed(dat);
                this._wait = 0;
            }
        }
    }
}
