// Decompiled with JetBrains decompiler
// Type: DuckGame.KnightHelmet
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
            _pickupSprite = new Sprite("knightHelmetPickup");
            _sprite = new SpriteMap("knightHelmet", 32, 32);
            graphic = _pickupSprite;
            center = new Vec2(8f, 8f);
            collisionOffset = new Vec2(-6f, -4f);
            collisionSize = new Vec2(11f, 12f);
            _equippedCollisionOffset = new Vec2(-4f, -2f);
            _equippedCollisionSize = new Vec2(11f, 12f);
            _hasEquippedCollision = true;
            _sprite.CenterOrigin();
            depth = (Depth)0.0001f;
            physicsMaterial = PhysicsMaterial.Metal;
            _isArmor = true;
            _equippedThickness = 3f;
            editorTooltip = "Protects ye olde medieval skull from impacts.";
        }

        public override void Update() => base.Update();
    }
}
