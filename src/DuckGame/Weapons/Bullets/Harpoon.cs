// Decompiled with JetBrains decompiler
// Type: DuckGame.Harpoon
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class Harpoon : Thing
    {
        public bool _inGun = true;
        public bool _stuck;
        //private float _hangGrav = 0.1f;
        //private float _hangPull;
        public Thing _belongsTo;
        public bool noisy = true;

        public ISwing swingOwner => _owner as ISwing;

        public bool inGun => _inGun;

        public bool stuck => _stuck;

        public override NetworkConnection connection => _belongsTo != null ? _belongsTo.connection : base.connection;

        public override NetIndex8 authority => _belongsTo != null ? _belongsTo.authority : base.authority;

        public Harpoon(Thing belongsTo = null)
          : base()
        {
            _belongsTo = belongsTo;
            owner = belongsTo;
            graphic = new Sprite("hook");
            center = new Vec2(3f, 3f);
            collisionOffset = new Vec2(-5f, -1.5f);
            collisionSize = new Vec2(10f, 5f);
        }

        public override void Update()
        {
            if (!isServerForObject)
                return;
            if (!_stuck)
                base.Update();
            else if (swingOwner != null)
            {
                Thing ropeParent = swingOwner.GetRopeParent(this);
            }
            if (!(_owner is Grapple) || !_inGun)
                return;
            Grapple owner = _owner as Grapple;
            position = owner.barrelPosition;
            depth = owner.depth - 1;
            hSpeed = 0f;
            vSpeed = 0f;
            graphic.flipH = owner.offDir < 0.0;
        }

        public void Latch(Vec2 point)
        {
            _inGun = false;
            position = point;
            _stuck = true;
        }

        public void SetStuckPoint(Vec2 pPoint)
        {
            _inGun = false;
            position = pPoint;
            _stuck = true;
        }

        public void Fire(Vec2 point, Vec2 travel)
        {
            if (!_inGun)
                return;
            _inGun = false;
            position = point + travel * -2f;
            _stuck = true;
            if (!noisy)
                return;
            SFX.Play("grappleHook", 0.5f);
            for (int index = 0; index < 6; ++index)
                Level.Add(Spark.New(point.x - travel.x * 2f, point.y - travel.y * 2f, travel));
            for (int index = 0; index < 1; ++index)
                Level.Add(SmallSmoke.New(point.x + Rando.Float(-2f, 2f), point.y + Rando.Float(-2f, 2f)));
        }

        public void Return()
        {
            if (_inGun)
                return;
            _inGun = true;
            hSpeed = 0f;
            vSpeed = 0f;
            _stuck = false;
        }

        public override void Draw()
        {
            if (inGun || !noisy)
                return;
            base.Draw();
        }
    }
}
