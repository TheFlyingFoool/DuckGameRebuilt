// Decompiled with JetBrains decompiler
// Type: DuckGame.Camera
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System;

namespace DuckGame
{
    public class Camera
    {
        protected Matrix _matrix;
        protected bool _dirty = true;
        protected Vec2 _position;
        protected Vec2 _size = new Vec2(320f, 320f * DuckGame.Graphics.aspect);
        protected Vec2 _zoomPoint = new Vec2(0f, 0f);
        public bool skipUpdate;
        private Rectangle _rectangle;
        public Vec2 _viewSize;

        public Vec2 position
        {
            get => this._position;
            set
            {
                if (!(this._position != value))
                    return;
                this._position = value;
                this._dirty = true;
            }
        }

        public float x
        {
            get => this._position.x;
            set
            {
                if (_position.x == value)
                    return;
                this._position.x = value;
                this._dirty = true;
            }
        }

        public float y
        {
            get => this._position.y;
            set
            {
                if (_position.y == value)
                    return;
                this._position.y = value;
                this._dirty = true;
            }
        }

        public Vec2 center
        {
            get => new Vec2(this.centerX, this.centerY);
            set
            {
                this.centerX = value.x;
                this.centerY = value.y;
            }
        }

        public float centerX
        {
            get => this._position.x + this.width / 2f;
            set
            {
                if (this.centerX == value)
                    return;
                this._position.x = value - this.width / 2f;
                this._dirty = true;
            }
        }

        public float centerY
        {
            get => this._position.y + this.height / 2f;
            set
            {
                if (this.centerY == value)
                    return;
                this._position.y = value - this.height / 2f;
                this._dirty = true;
            }
        }

        public float aspect => this.width / this.height;

        public virtual bool sixteenNine => Math.Abs(this.aspect - 1.777778f) < 0.02f;

        public void InitializeToScreenAspect()
        {
            this._size = new Vec2(320f, 320f / Resolution.current.aspect);
            this._dirty = true;
        }

        public float top => this.y;

        public float bottom => this.y + this.height;

        public float left => this.x;

        public float right => this.x + this.width;

        public Vec2 size
        {
            get => this._size;
            set
            {
                this._size = value;
                this._dirty = true;
            }
        }

        public float width
        {
            get => this._size.x;
            set
            {
                if (_size.x == value)
                    return;
                this._size.x = value;
                this._dirty = true;
            }
        }

        public float height
        {
            get => this._size.y;
            set
            {
                if (_size.y == value)
                    return;
                this._size.y = value;
                this._dirty = true;
            }
        }

        public Vec2 zoomPoint
        {
            get => this._zoomPoint;
            set => this._zoomPoint = value;
        }

        public float PercentW(float percent) => this.width * (percent / 100f);

        public float PercentH(float percent) => this.height * (percent / 100f);

        public Vec2 PercentWH(float wide, float high) => new Vec2(this.width * (wide / 100f), this.height * (high / 100f));

        public Vec2 OffsetTL(float t, float l) => new Vec2(this.x + t, this.y + l);

        public Vec2 OffsetBR(float t, float l) => new Vec2(this.x + this.width + t, this.y + this.height + l);

        public Vec2 OffsetCenter(float t, float l) => new Vec2(this.x + this.PercentW(50f) + t, this.y + this.PercentH(50f) + l);

        public Camera()
        {
        }

        public Camera(float xval, float yval, float wval = -1f, float hval = -1f)
        {
            if (wval < 0f)
                wval = 320f;
            if (hval < 0f)
                hval = 320f * DuckGame.Graphics.aspect;
            this.x = xval;
            this.y = yval;
            this.width = wval;
            this.height = hval;
        }

        public void DoUpdate()
        {
            if (this.skipUpdate)
                this.skipUpdate = false;
            else
                this.Update();
        }

        public virtual void Update()
        {
        }

        public virtual Vec2 transformScreenVector(Vec2 vector)
        {
            Vec3 vec3 = Vec3.Transform(new Vec3(vector.x, vector.y, 0f), Matrix.Invert(this.getMatrix()));
            return new Vec2(vec3.x, vec3.y);
        }

        public virtual Vec2 transformTime(Vec2 vector)
        {
            Vec3 vec3 = Vec3.Transform(new Vec3(vector.x, vector.y, 0f), Resolution.getTransformationMatrix() * this.getMatrix());
            return new Vec2(vec3.x, vec3.y);
        }

        public virtual Vec2 transformWorldVector(Vec2 vector)
        {
            Vec3 vec3 = Vec3.Transform(new Vec3(vector.x, vector.y, 0f), Matrix.Invert(Resolution.getTransformationMatrix()) * this.getMatrix());
            return new Vec2(vec3.x, vec3.y);
        }

        public virtual Vec2 transform(Vec2 vector)
        {
            Vec3 vec3 = Vec3.Transform(new Vec3(vector.x, vector.y, 0f), this.getMatrix());
            return new Vec2(vec3.x, vec3.y);
        }

        public virtual Vec2 transformInverse(Vec2 vector)
        {
            Vec3 vec3 = Vec3.Transform(new Vec3(vector.x, vector.y, 0f), Matrix.Invert(this.getMatrix()));
            return new Vec2(vec3.x, vec3.y);
        }

        public Rectangle rectangle => this._rectangle;

        public virtual Matrix getMatrix()
        {
            Viewport viewport;
            if (!this._dirty)
            {
                viewport = DuckGame.Graphics.viewport;
                if (viewport.Width == this._viewSize.x)
                {
                    viewport = DuckGame.Graphics.viewport;
                    if (viewport.Height == this._viewSize.y)
                        goto label_4;
                }
            }
            this._rectangle = new Rectangle(this.left - 16f, this.top - 16f, this.size.x + 32f, this.size.y + 32f);
            viewport = DuckGame.Graphics.viewport;
            double width1 = viewport.Width;
            viewport = DuckGame.Graphics.viewport;
            double height1 = viewport.Height;
            this._viewSize = new Vec2((float)width1, (float)height1);
            Vec2 position = this.position;
            float width2 = this.width;
            float height2 = this.height;
            this._matrix = Matrix.CreateTranslation(new Vec3(-position.x, -position.y, 0f)) * Matrix.CreateScale(this._viewSize.x / width2, this._viewSize.y / height2, 1f);
            this._dirty = false;
        label_4:
            return this._matrix;
        }
    }
}
