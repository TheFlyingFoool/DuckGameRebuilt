// Decompiled with JetBrains decompiler
// Type: DuckGame.Hat
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

namespace DuckGame
{
    public abstract class Hat : Equipment
    {
        protected Vec2 _hatOffset = Vec2.Zero;
        public bool strappedOn;
        public bool quacks = true;
        protected SpriteMap _sprite;
        protected Sprite _pickupSprite;
        protected bool _hasUnequippedCenter;

        public Vec2 hatOffset
        {
            get => this._hatOffset;
            set => this._hatOffset = value;
        }

        public virtual void SetQuack(int pValue) => this.frame = pValue;

        public SpriteMap sprite
        {
            get => this._sprite;
            set => this._sprite = value;
        }

        public Sprite pickupSprite
        {
            get => this._pickupSprite;
            set => this._pickupSprite = value;
        }

        public Hat(float xpos, float ypos)
          : base(xpos, ypos)
        {
            this.center = new Vec2(16f, 16f);
            this.collisionOffset = new Vec2(-6f, -6f);
            this.collisionSize = new Vec2(12f, 12f);
            this._autoOffset = false;
            this.thickness = 0.1f;
            this._sprite = new SpriteMap("hats/burgers", 32, 32);
            this._pickupSprite = new SpriteMap("hats/burgers", 32, 32);
            this._equippedDepth = 6;
        }

        public virtual void Quack(float volume, float pitch) => SFX.Play("quack", volume, pitch);

        public virtual void OpenHat()
        {
        }

        public virtual void CloseHat()
        {
        }

        public override void Update()
        {
            if (this._equippedDuck != null && !this.destroyed)
            {
                this.center = new Vec2(this._sprite.w / 2, this._sprite.h / 2);
                this.graphic = _sprite;
                this.solid = false;
                this.visible = false;
            }
            else
            {
                this._sprite.frame = 0;
                if (!this._hasUnequippedCenter)
                    this.center = new Vec2(this._pickupSprite.w / 2, this._pickupSprite.h / 2);
                this.graphic = this._pickupSprite;
                this.solid = true;
                this._sprite.flipH = false;
                this.visible = true;
            }
            if (this.destroyed)
                this.alpha -= 0.05f;
            if (this.alpha < 0.0)
                Level.Remove(this);
            base.Update();
            if (this._equippedDuck != null && this._equippedDuck._trapped != null)
            {
                this.depth = this._equippedDuck._trapped.depth + 2;
            }
            else
            {
                if (this.owner != null || !(this is TeamHat))
                    return;
                this.depth = -0.2f;
            }
        }
    }
}
