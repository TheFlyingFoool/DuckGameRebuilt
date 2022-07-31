// Decompiled with JetBrains decompiler
// Type: DuckGame.WaterRifle
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [BaggedProperty("canSpawn", false)]
    [BaggedProperty("isFatal", false)]
    public class WaterRifle : Gun
    {
        private FluidStream _stream;
        private ConstantSound _sound;
        private int _wait;

        public WaterRifle(float xval, float yval)
          : base(xval, yval)
        {
            this.ammo = 9;
            this._ammoType = new AT9mm();
            this._type = "gun";
            this.graphic = new Sprite("waterGun");
            this.center = new Vec2(11f, 7f);
            this.collisionOffset = new Vec2(-11f, -6f);
            this.collisionSize = new Vec2(23f, 13f);
            this._barrelOffsetTL = new Vec2(24f, 6f);
            this._fireSound = "pistolFire";
            this._kickForce = 3f;
            this._holdOffset = new Vec2(-1f, 0f);
            this.loseAccuracy = 0.1f;
            this.maxAccuracyLost = 0.6f;
            this._bio = "";
            this._editorName = "Water Blaster";
            this.physicsMaterial = PhysicsMaterial.Metal;
            this._stream = new FluidStream(this.x, this.y, new Vec2(1f, 0f), 2f);
            this.isFatal = false;
        }

        public override void Initialize()
        {
            _sound = new ConstantSound("demoBlaster");
            Level.Add(_stream);
        }

        public override void Terminate() => Level.Remove(_stream);

        public override void Update() => base.Update();

        public override void OnPressAction()
        {
        }

        public override void OnHoldAction()
        {
            ++this._wait;
            if (this._wait != 3)
                return;
            this._stream.sprayAngle = this.barrelVector * 2f;
            this._stream.position = this.barrelPosition;
            FluidData dat = Fluid.Water;
            dat.amount = 0.01f;
            this._stream.Feed(dat);
            this._wait = 0;
        }
    }
}
