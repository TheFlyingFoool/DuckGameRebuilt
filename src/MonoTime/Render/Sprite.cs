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

        public int globalIndex => this._globalIndex;

        public Tex2D texture
        {
            get => this._texture;
            set => this._texture = value;
        }

        public RenderTarget2D renderTexture
        {
            get => this._renderTexture;
            set => this._renderTexture = value;
        }

        public virtual int width => this._texture.width;

        public virtual int w => this.width;

        public virtual int height => this._texture.height;

        public virtual int h => this.height;

        public bool flipH
        {
            get => this._flipH;
            set => this._flipH = value;
        }

        public bool flipV
        {
            get => this._flipV;
            set => this._flipV = value;
        }

        public float flipMultH => !this._flipH ? 1f : -1f;

        public float flipMultV => !this._flipV ? 1f : -1f;

        public Color color
        {
            get => this._color;
            set => this._color = value;
        }

        public void CenterOrigin() => this.center = new Vec2((float)Math.Round(width / 2.0), (float)Math.Round(height / 2.0));

        public Sprite()
        {
        }

        public Sprite(Tex2D tex, float x = 0f, float y = 0f)
        {
            this._texture = tex;
            this.position = new Vec2(x, y);
        }

        public Sprite(RenderTarget2D tex, float x = 0f, float y = 0f)
        {
            this._texture = tex;
            this._renderTexture = tex;
            this.position = new Vec2(x, y);
        }

        public Sprite(string tex, float x = 0f, float y = 0f)
        {
            this._texture = Content.Load<Tex2D>(tex);
            this.position = new Vec2(x, y);
        }

        public Sprite(string tex, Vec2 pCenter)
        {
            this._texture = Content.Load<Tex2D>(tex);
            this.center = pCenter;
        }

        public virtual void Draw()
        {
            this._texture.currentObjectIndex = this._globalIndex;
            DuckGame.Graphics.Draw(this._texture, this.position, new Rectangle?(), this._color * this.alpha, this.angle, this.center, this.scale, this._flipH ? SpriteEffects.FlipHorizontally : (this._flipV ? SpriteEffects.FlipVertically : SpriteEffects.None), this.depth);
        }

        public virtual void Draw(Rectangle r)
        {
            this._texture.currentObjectIndex = this._globalIndex;
            DuckGame.Graphics.Draw(this._texture, this.position, new Rectangle?(r), this._color * this.alpha, this.angle, this.center, this.scale, this._flipH ? SpriteEffects.FlipHorizontally : (this._flipV ? SpriteEffects.FlipVertically : SpriteEffects.None), this.depth);
        }

        public virtual void CheapDraw(bool flipH)
        {
        }

        public virtual Sprite Clone()
        {
            Sprite sprite = new Sprite(this._texture)
            {
                flipH = this._flipH,
                flipV = this._flipV,
                position = this.position,
                scale = this.scale,
                center = this.center,
                depth = this.depth,
                alpha = this.alpha,
                angle = this.angle,
                color = this.color
            };
            return sprite;
        }

        public virtual void UltraCheapStaticDraw(bool flipH)
        {
        }

        object ICloneable.Clone() => this.Clone();
    }
}
