// Decompiled with JetBrains decompiler
// Type: DuckGame.UIDivider
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;

namespace DuckGame
{
    public class UIDivider : UIComponent
    {
        private float _splitPercent;
        private int _splitPixels = -1;
        private UIBox _leftBox;
        private UIBox _rightBox;
        private float _seperation = 1f;

        public UIBox leftSection => this._leftBox;

        public UIBox rightSection => this._rightBox;

        public UIBox topSection => this._leftBox;

        public UIBox bottomSection => this._rightBox;

        public UIDivider(bool vert, float splitVal, float sep = 1f)
          : base(0.0f, 0.0f, 0.0f, 0.0f)
        {
            this._vertical = vert;
            this._splitPercent = splitVal;
            this._leftBox = new UIBox(isVisible: false);
            this._rightBox = new UIBox(isVisible: false);
            this.Add((UIComponent)this._leftBox);
            this.Add((UIComponent)this._rightBox);
            this._canFit = true;
            this._seperation = sep;
        }

        public UIDivider(bool vert, int splitVal, float sep = 1f)
          : base(0.0f, 0.0f, 0.0f, 0.0f)
        {
            this._vertical = vert;
            this._leftBox = new UIBox(isVisible: false);
            this._rightBox = new UIBox(isVisible: false);
            this.Add((UIComponent)this._leftBox);
            this.Add((UIComponent)this._rightBox);
            this._splitPixels = splitVal;
            this._canFit = true;
            this._seperation = sep;
        }

        protected override void SizeChildren()
        {
            if (this._vertical)
            {
                Vec2 sizes = this.CalculateSizes();
                this._leftBox.collisionSize = new Vec2(sizes.x, this.collisionSize.y - this.borderSize.y * 2f);
                this._rightBox.collisionSize = new Vec2(sizes.y, this.collisionSize.y - this.borderSize.y * 2f);
            }
            else
            {
                Vec2 sizes = this.CalculateSizes();
                this._leftBox.collisionSize = new Vec2(this.collisionSize.x - this.borderSize.x * 2f, sizes.x);
                this._rightBox.collisionSize = new Vec2(this.collisionSize.x - this.borderSize.x * 2f, sizes.y);
            }
        }

        public Vec2 CalculateSizes()
        {
            Vec2 collisionSize = this.collisionSize;
            if (this._vertical)
            {
                collisionSize.x -= this._seperation;
                float x = this._leftBox.collisionSize.x;
                float y = this._rightBox.collisionSize.x;
                if ((double)this._splitPercent != 0.0)
                {
                    x = collisionSize.x * this._splitPercent;
                    y = collisionSize.x * (1f - this._splitPercent);
                }
                else if (this._splitPixels > 0)
                {
                    x = (float)this._splitPixels;
                    y = collisionSize.x - (float)this._splitPixels;
                }
                return new Vec2(x, y);
            }
            collisionSize.y -= this._seperation;
            float x1 = this._leftBox.collisionSize.y;
            float y1 = this._rightBox.collisionSize.y;
            if ((double)this._splitPercent != 0.0)
            {
                x1 = collisionSize.y * this._splitPercent;
                y1 = collisionSize.y * (1f - this._splitPercent);
            }
            else if (this._splitPixels > 0)
            {
                x1 = (float)this._splitPixels;
                y1 = collisionSize.y - (float)this._splitPixels;
            }
            return new Vec2(x1, y1);
        }

        protected override void OnResize()
        {
            if (this._vertical)
            {
                this._collisionSize.y = Math.Max(this._leftBox.collisionSize.y, this._rightBox.collisionSize.y);
                float num1 = this._leftBox.collisionSize.x + this._rightBox.collisionSize.x + this._seperation;
                if ((double)this._collisionSize.x < (double)num1)
                    this._collisionSize.x = num1;
                Vec2 sizes = this.CalculateSizes();
                float num2 = sizes.x;
                float num3 = sizes.y;
                if ((double)num2 < (double)this._leftBox.collisionSize.x)
                    num2 = this._leftBox.collisionSize.x;
                if ((double)num3 < (double)this._rightBox.collisionSize.x)
                {
                    num3 = this._rightBox.collisionSize.x;
                    num2 = this._collisionSize.x - num3;
                }
                float num4 = Math.Max(this._leftBox.collisionSize.y, this._rightBox.collisionSize.y);
                if ((double)this._collisionSize.y < (double)num4)
                    this._collisionSize.y = num4;
                this._leftBox.anchor.offset.x = (float)(-(double)this.halfWidth + (double)num2 / 2.0);
                this._leftBox.anchor.offset.y = 0.0f;
                this._rightBox.anchor.offset.x = this.halfWidth - num3 / 2f;
                this._rightBox.anchor.offset.y = 0.0f;
            }
            else
            {
                this._collisionSize.y = Math.Max(this._leftBox.collisionSize.y, (float)this._splitPixels) + this._rightBox.collisionSize.y;
                float num5 = this._leftBox.collisionSize.y + this._rightBox.collisionSize.y + this._seperation;
                if ((double)this._collisionSize.y < (double)num5)
                    this._collisionSize.y = num5;
                Vec2 sizes = this.CalculateSizes();
                float num6 = sizes.x;
                float y = sizes.y;
                if ((double)num6 < (double)this._leftBox.collisionSize.y)
                    num6 = this._leftBox.collisionSize.y;
                if ((double)y < (double)this._rightBox.collisionSize.y)
                {
                    y = this._rightBox.collisionSize.y;
                    num6 = this._collisionSize.y - y;
                }
                float num7 = Math.Max(this._leftBox.collisionSize.x, this._rightBox.collisionSize.x);
                if ((double)this._collisionSize.x < (double)num7)
                    this._collisionSize.x = num7;
                this._leftBox.anchor.offset.x = 0.0f;
                this._leftBox.anchor.offset.y = (float)(-(double)this.halfHeight + (double)num6 / 2.0);
                this._rightBox.anchor.offset.x = 0.0f;
                this._rightBox.anchor.offset.y = this.halfHeight - y / 2f;
            }
        }

        public override void Draw()
        {
            if (!this._vertical)
            {
                Vec2 vec2_1 = this._rightBox.position - new Vec2(this._rightBox.width / 2f, this._rightBox.height / 2f);
                Vec2 vec2_2 = this._leftBox.position - new Vec2(this._leftBox.width / 2f, this._leftBox.height / 2f);
                if ((double)vec2_2.x < (double)vec2_1.x)
                    vec2_1.x = vec2_2.x;
                vec2_1.y -= this._seperation / 2f;
                Vec2 vec2_3 = this._rightBox.position + new Vec2(this._rightBox.width / 2f, this._rightBox.height / 2f);
                Vec2 vec2_4 = this._leftBox.position + new Vec2(this._leftBox.width / 2f, this._leftBox.height / 2f);
                if ((double)vec2_4.x > (double)vec2_3.x)
                    vec2_3.x = vec2_4.x;
                if (this._splitPixels == 0)
                {
                    double splitPercent = (double)this._splitPercent;
                }
                else
                {
                    int splitPixels = this._splitPixels;
                }
                Graphics.DrawLine(new Vec2(vec2_1.x, vec2_1.y), new Vec2(vec2_3.x, vec2_1.y), Color.White, depth: (this.depth + 10));
            }
            int num = this.debug ? 1 : 0;
            base.Draw();
        }
    }
}
