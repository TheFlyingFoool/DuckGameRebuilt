// Decompiled with JetBrains decompiler
// Type: DuckGame.GrappleHook
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public class GrappleHook : PhysicsObject
    {
        private Grapple _owner;
        private bool _inGun = true;
        private bool _stuck;

        public bool inGun => _inGun;

        public GrappleHook(Grapple ownerVal)
          : base(0f, 0f)
        {
            _owner = ownerVal;
            graphic = new Sprite("harpoon");
            center = new Vec2(8f, 8f);
            collisionOffset = new Vec2(-5f, -1.5f);
            collisionSize = new Vec2(10f, 5f);
        }

        public override void Update()
        {
            if (!_stuck)
                base.Update();
            if (!_inGun || _owner == null)
                return;
            position = _owner.barrelPosition;
            depth = _owner.depth - 1;
            hSpeed = 0f;
            vSpeed = 0f;
            graphic.flipH = _owner.offDir < 0;
        }

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (_inGun || !(with is Block))
                return;
            _stuck = true;
        }

        public void Fire()
        {
            if (!_inGun || _owner == null)
                return;
            _inGun = false;
            hSpeed = _owner.offDir * 6f;
            vSpeed = -8f;
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

        public override void Draw() => base.Draw();
    }
}
