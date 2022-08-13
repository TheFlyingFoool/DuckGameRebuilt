// Decompiled with JetBrains decompiler
// Type: DuckGame.Helmet
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    [EditorGroup("Equipment")]
    [BaggedProperty("isInDemo", true)]
    [BaggedProperty("previewPriority", true)]
    public class Helmet : Hat
    {
        public StateBinding _crushedBinding = new StateBinding(nameof(crushed));
        public bool crushed;

        public Helmet(float xpos, float ypos)
          : base(xpos, ypos)
        {
            _pickupSprite = new Sprite("helmetPickup");
            _sprite = new SpriteMap("helmet", 32, 32);
            graphic = _pickupSprite;
            center = new Vec2(8f, 8f);
            collisionOffset = new Vec2(-5f, -2f);
            collisionSize = new Vec2(12f, 8f);
            _sprite.CenterOrigin();
            _isArmor = true;
            _equippedThickness = 3f;
            editorTooltip = "Protects your precious, precious brain from impacts.";
        }

        public virtual void Crush() => crushed = true;

        public virtual void Crush(Thing pWith) => crushed = true;

        public override void Update()
        {
            if (_equippedDuck != null)
            {
                if (_equippedDuck.sliding)
                {
                    collisionOffset = new Vec2(-3f, -7f);
                    collisionSize = new Vec2(9f, 12f);
                }
                else
                {
                    collisionOffset = new Vec2(-5f, -2f);
                    collisionSize = new Vec2(12f, 8f);
                }
            }
            else
            {
                collisionOffset = new Vec2(-5f, -2f);
                collisionSize = new Vec2(12f, 8f);
            }
            base.Update();
        }

        public override void Draw()
        {
            int frame = _sprite.frame;
            _sprite.frame = crushed ? 1 : 0;
            base.Draw();
            _sprite.frame = frame;
        }
    }
}
