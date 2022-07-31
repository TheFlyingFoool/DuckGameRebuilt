// Decompiled with JetBrains decompiler
// Type: DuckGame.GrappleHook
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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

        public bool inGun => this._inGun;

        public GrappleHook(Grapple ownerVal)
          : base(0f, 0f)
        {
            this._owner = ownerVal;
            this.graphic = new Sprite("harpoon");
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-5f, -1.5f);
            this.collisionSize = new Vec2(10f, 5f);
        }

        public override void Update()
        {
            if (!this._stuck)
                base.Update();
            if (!this._inGun || this._owner == null)
                return;
            this.position = this._owner.barrelPosition;
            this.depth = this._owner.depth - 1;
            this.hSpeed = 0f;
            this.vSpeed = 0f;
            this.graphic.flipH = _owner.offDir < 0.0;
        }

        public override void OnSoftImpact(MaterialThing with, ImpactedFrom from)
        {
            if (this._inGun || !(with is Block))
                return;
            this._stuck = true;
        }

        public void Fire()
        {
            if (!this._inGun || this._owner == null)
                return;
            this._inGun = false;
            this.hSpeed = _owner.offDir * 6f;
            this.vSpeed = -8f;
        }

        public void Return()
        {
            if (this._inGun)
                return;
            this._inGun = true;
            this.hSpeed = 0f;
            this.vSpeed = 0f;
            this._stuck = false;
        }

        public override void Draw() => base.Draw();
    }
}
