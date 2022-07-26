// Decompiled with JetBrains decompiler
// Type: DuckGame.KnightHelmet
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Equipment")]
    public class KnightHelmet : Helmet
    {
        public KnightHelmet(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this._pickupSprite = new Sprite("knightHelmetPickup");
            this._sprite = new SpriteMap("knightHelmet", 32, 32);
            this.graphic = this._pickupSprite;
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-6f, -4f);
            this.collisionSize = new Vec2(11f, 12f);
            this._equippedCollisionOffset = new Vec2(-4f, -2f);
            this._equippedCollisionSize = new Vec2(11f, 12f);
            this._hasEquippedCollision = true;
            this._sprite.CenterOrigin();
            this.depth = (Depth)0.0001f;
            this.physicsMaterial = PhysicsMaterial.Metal;
            this._isArmor = true;
            this._equippedThickness = 3f;
            this.editorTooltip = "Protects ye olde medieval skull from impacts.";
        }

        public override void Update() => base.Update();
    }
}
