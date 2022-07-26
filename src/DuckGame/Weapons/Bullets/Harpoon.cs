// Decompiled with JetBrains decompiler
// Type: DuckGame.Harpoon
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class Harpoon : Thing
    {
        public bool _inGun = true;
        public bool _stuck;
        private float _hangGrav = 0.1f;
        private float _hangPull;
        public Thing _belongsTo;
        public bool noisy = true;

        public ISwing swingOwner => this._owner as ISwing;

        public bool inGun => this._inGun;

        public bool stuck => this._stuck;

        public override NetworkConnection connection => this._belongsTo != null ? this._belongsTo.connection : base.connection;

        public override NetIndex8 authority => this._belongsTo != null ? this._belongsTo.authority : base.authority;

        public Harpoon(Thing belongsTo = null)
          : base()
        {
            this._belongsTo = belongsTo;
            this.owner = belongsTo;
            this.graphic = new Sprite("hook");
            this.center = new Vec2(3f, 3f);
            this.collisionOffset = new Vec2(-5f, -1.5f);
            this.collisionSize = new Vec2(10f, 5f);
        }

        public override void Update()
        {
            if (!this.isServerForObject)
                return;
            if (!this._stuck)
                base.Update();
            else if (this.swingOwner != null)
            {
                Thing ropeParent = (Thing)this.swingOwner.GetRopeParent((Thing)this);
            }
            if (!(this._owner is Grapple) || !this._inGun)
                return;
            Grapple owner = this._owner as Grapple;
            this.position = owner.barrelPosition;
            this.depth = owner.depth - 1;
            this.hSpeed = 0.0f;
            this.vSpeed = 0.0f;
            this.graphic.flipH = (double)owner.offDir < 0.0;
        }

        public void Latch(Vec2 point)
        {
            this._inGun = false;
            this.position = point;
            this._stuck = true;
        }

        public void SetStuckPoint(Vec2 pPoint)
        {
            this._inGun = false;
            this.position = pPoint;
            this._stuck = true;
        }

        public void Fire(Vec2 point, Vec2 travel)
        {
            if (!this._inGun)
                return;
            this._inGun = false;
            this.position = point + travel * -2f;
            this._stuck = true;
            if (!this.noisy)
                return;
            SFX.Play("grappleHook", 0.5f);
            for (int index = 0; index < 6; ++index)
                Level.Add((Thing)Spark.New(point.x - travel.x * 2f, point.y - travel.y * 2f, travel));
            for (int index = 0; index < 1; ++index)
                Level.Add((Thing)SmallSmoke.New(point.x + Rando.Float(-2f, 2f), point.y + Rando.Float(-2f, 2f)));
        }

        public void Return()
        {
            if (this._inGun)
                return;
            this._inGun = true;
            this.hSpeed = 0.0f;
            this.vSpeed = 0.0f;
            this._stuck = false;
        }

        public override void Draw()
        {
            if (this.inGun || !this.noisy)
                return;
            base.Draw();
        }
    }
}
