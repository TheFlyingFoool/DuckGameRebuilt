// Decompiled with JetBrains decompiler
// Type: DuckGame.Sprite
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System;

namespace DuckGame
{
    public class Sprite : Transform, ICloneable<Sprite>, ICloneable
    {
        private int _globalIndex = Thing.GetGlobalIndex();
        protected Tex2D _texture;
        protected RenderTarget2D _renderTexture;
        protected bool _flipH;
        protected bool _flipV;
        public bool moji;
        protected Color _color = Color.White;

        public int globalIndex => _globalIndex;

        public Tex2D texture
        {
            get => _texture;
            set => _texture = value;
        }

        public RenderTarget2D renderTexture
        {
            get => _renderTexture;
            set => _renderTexture = value;
        }

        public virtual int width => _texture.width;

        public virtual int w => width;

        public virtual int height => _texture.height;

        public virtual int h => height;

        public bool flipH
        {
            get => _flipH;
            set => _flipH = value;
        }

        public bool flipV
        {
            get => _flipV;
            set => _flipV = value;
        }

        public float flipMultH => !_flipH ? 1f : -1f;

        public float flipMultV => !_flipV ? 1f : -1f;

        public Color color
        {
            get => _color;
            set => _color = value;
        }

        public void CenterOrigin() => center = new Vec2((float)Math.Round(width / 2.0), (float)Math.Round(height / 2.0));

        public Sprite()
        {
        }

        public Sprite(Tex2D tex, float x = 0f, float y = 0f)
        {
            _texture = tex;
            position = new Vec2(x, y);
        }

        public Sprite(RenderTarget2D tex, float x = 0f, float y = 0f)
        {
            _texture = tex;
            _renderTexture = tex;
            position = new Vec2(x, y);
        }

        public Sprite(string tex, float x = 0f, float y = 0f)
        {
            _texture = Content.Load<Tex2D>(tex);
            position = new Vec2(x, y);
        }

        public Sprite(string tex, Vec2 pCenter)
        {
            _texture = Content.Load<Tex2D>(tex);
            center = pCenter;
        }

        public virtual void Draw()
        {
            _texture.currentObjectIndex = _globalIndex;
            DuckGame.Graphics.Draw(_texture, position, new Rectangle?(), _color * alpha, angle, center, scale, _flipH ? SpriteEffects.FlipHorizontally : (_flipV ? SpriteEffects.FlipVertically : SpriteEffects.None), depth);
        }

        public virtual void Draw(Rectangle r)
        {
            _texture.currentObjectIndex = _globalIndex;
            DuckGame.Graphics.Draw(_texture, position, new Rectangle?(r), _color * alpha, angle, center, scale, _flipH ? SpriteEffects.FlipHorizontally : (_flipV ? SpriteEffects.FlipVertically : SpriteEffects.None), depth);
        }

        public virtual void CheapDraw(bool flipH)
        {
        }

        public virtual Sprite Clone()
        {
            Sprite sprite = new Sprite(_texture)
            {
                flipH = _flipH,
                flipV = _flipV,
                position = position,
                scale = scale,
                center = center,
                depth = depth,
                alpha = alpha,
                angle = angle,
                color = color
            };
            return sprite;
        }

        public virtual void UltraCheapStaticDraw(bool flipH)
        {
        }

        object ICloneable.Clone() => Clone();
    }
}
