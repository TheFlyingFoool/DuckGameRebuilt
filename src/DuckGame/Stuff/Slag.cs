// Decompiled with JetBrains decompiler
// Type: DuckGame.Slag
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Stuff|Props")]
    [BaggedProperty("isInDemo", true)]
    public class Slag : Holdable, IPlatform
    {
        private SpriteMap _sprite;

        public Slag(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this._sprite = new SpriteMap("slag", 16, 16);
            this.graphic = _sprite;
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-8f, -8f);
            this.collisionSize = new Vec2(16f, 16f);
            this.depth = -0.5f;
            this.thickness = 4f;
            this.weight = 7f;
            this.flammable = 0.0f;
            this.collideSounds.Add("rockHitGround2");
            this.physicsMaterial = PhysicsMaterial.Metal;
            this.buoyancy = 1f;
            this.onlyFloatInLava = true;
            this.editorTooltip = "What a WORD! Floats in Lava and doesn't burn.";
        }

        public override void Update()
        {
            this.heat = 0.0f;
            base.Update();
        }

        public override bool Hit(Bullet bullet, Vec2 hitPos)
        {
            if (bullet.isLocal && this.owner == null)
                Thing.Fondle(this, DuckNetwork.localConnection);
            if (this.isServerForObject && bullet.isLocal && TeamSelect2.Enabled("EXPLODEYCRATES"))
            {
                if (this.duck != null)
                    this.duck.ThrowItem();
                this.Destroy(new DTShot(bullet));
                Level.Remove(this);
                Level.Add(new GrenadeExplosion(this.x, this.y));
            }
            return base.Hit(bullet, hitPos);
        }
    }
}
