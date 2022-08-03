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
            get => _hatOffset;
            set => _hatOffset = value;
        }

        public virtual void SetQuack(int pValue) => frame = pValue;

        public SpriteMap sprite
        {
            get => _sprite;
            set => _sprite = value;
        }

        public Sprite pickupSprite
        {
            get => _pickupSprite;
            set => _pickupSprite = value;
        }

        public Hat(float xpos, float ypos)
          : base(xpos, ypos)
        {
            center = new Vec2(16f, 16f);
            collisionOffset = new Vec2(-6f, -6f);
            collisionSize = new Vec2(12f, 12f);
            _autoOffset = false;
            thickness = 0.1f;
            _sprite = new SpriteMap("hats/burgers", 32, 32);
            _pickupSprite = new SpriteMap("hats/burgers", 32, 32);
            _equippedDepth = 6;
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
            if (_equippedDuck != null && !destroyed)
            {
                center = new Vec2(_sprite.w / 2, _sprite.h / 2);
                graphic = _sprite;
                solid = false;
                visible = false;
            }
            else
            {
                _sprite.frame = 0;
                if (!_hasUnequippedCenter)
                    center = new Vec2(_pickupSprite.w / 2, _pickupSprite.h / 2);
                graphic = _pickupSprite;
                solid = true;
                _sprite.flipH = false;
                visible = true;
            }
            if (destroyed)
                alpha -= 0.05f;
            if (alpha < 0.0)
                Level.Remove(this);
            base.Update();
            if (_equippedDuck != null && _equippedDuck._trapped != null)
            {
                depth = _equippedDuck._trapped.depth + 2;
            }
            else
            {
                if (owner != null || !(this is TeamHat))
                    return;
                depth = -0.2f;
            }
        }
    }
}
