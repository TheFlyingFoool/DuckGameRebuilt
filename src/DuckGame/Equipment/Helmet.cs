// Decompiled with JetBrains decompiler
// Type: DuckGame.Helmet
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
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
            this._pickupSprite = new Sprite("helmetPickup");
            this._sprite = new SpriteMap("helmet", 32, 32);
            this.graphic = this._pickupSprite;
            this.center = new Vec2(8f, 8f);
            this.collisionOffset = new Vec2(-5f, -2f);
            this.collisionSize = new Vec2(12f, 8f);
            this._sprite.CenterOrigin();
            this._isArmor = true;
            this._equippedThickness = 3f;
            this.editorTooltip = "Protects your precious, precious brain from impacts.";
        }

        public virtual void Crush() => this.crushed = true;

        public virtual void Crush(Thing pWith) => this.crushed = true;

        public override void Update()
        {
            if (this._equippedDuck != null)
            {
                if (this._equippedDuck.sliding)
                {
                    this.collisionOffset = new Vec2(-3f, -7f);
                    this.collisionSize = new Vec2(9f, 12f);
                }
                else
                {
                    this.collisionOffset = new Vec2(-5f, -2f);
                    this.collisionSize = new Vec2(12f, 8f);
                }
            }
            else
            {
                this.collisionOffset = new Vec2(-5f, -2f);
                this.collisionSize = new Vec2(12f, 8f);
            }
            base.Update();
        }

        public override void Draw()
        {
            int frame = this._sprite.frame;
            this._sprite.frame = this.crushed ? 1 : 0;
            base.Draw();
            this._sprite.frame = frame;
        }
    }
}
