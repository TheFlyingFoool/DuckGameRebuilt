// Decompiled with JetBrains decompiler
// Type: DuckGame.Camera
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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
        protected Vec2 _size = new Vec2(320f, 320f * Graphics.aspect);
        protected Vec2 _zoomPoint = new Vec2(0f, 0f);
        public bool skipUpdate;
        private Rectangle _rectangle;
        public Vec2 _viewSize;
        protected Interp CameraLerp = new Interp(false);
        public bool LerpWithoutFear = false;

        public Vec2 position
        {
            get => _position;
            set
            {
                if (!(_position != value))
                    return;
                _position = value;
                _dirty = true;
                if (DGRSettings.UncappedFPS)
                    SubFrameUpdate();
            }
        }

        public float x
        {
            get => _position.x;
            set
            {
                if (_position.x == value)
                    return;
                _position.x = value;
                _dirty = true;
                if (DGRSettings.UncappedFPS)
                    SubFrameUpdate();
            }
        }

        public float y
        {
            get => _position.y;
            set
            {
                if (_position.y == value)
                    return;
                _position.y = value;
                _dirty = true; 
                if (DGRSettings.UncappedFPS)
                    SubFrameUpdate();
            }
        }

        public Vec2 center
        {
            get => new Vec2(centerX, centerY);
            set
            {
                centerX = value.x;
                centerY = value.y;
                if (DGRSettings.UncappedFPS)
                    SubFrameUpdate();
            }
        }

        public float centerX
        {
            get => x + width / 2f;
            set
            {
                if (centerX == value)
                    return;
                _position.x = value - width / 2f;
                _dirty = true;
                if (DGRSettings.UncappedFPS)
                    SubFrameUpdate();
            }
        }

        public float centerY
        {
            get => y + height / 2f;
            set
            {
                if (centerY == value)
                    return;
                _position.y = value - height / 2f;
                _dirty = true;
                if (DGRSettings.UncappedFPS)
                    SubFrameUpdate();
            }
        }

        public float aspect => width / height;

        public virtual bool sixteenNine => Math.Abs(aspect - 1.777778f) < 0.02f;

        public void InitializeToScreenAspect()
        {
            _size = new Vec2(320f, 320f / Resolution.current.aspect);
            _dirty = true;
        }

        public float top => y;

        public float bottom => y + height;

        public float left => x;

        public float right => x + width;

        public Vec2 size
        {
            get => _size;
            set
            {
                _size = value;
                _dirty = true;
            }
        }

        public float width
        {
            get => _size.x;
            set
            {
                if (_size.x == value)
                    return;
                _size.x = value;
                _dirty = true;
            }
        }

        public float height
        {
            get => _size.y;
            set
            {
                if (_size.y == value)
                    return;
                _size.y = value;
                _dirty = true;
            }
        }

        public Vec2 zoomPoint
        {
            get => _zoomPoint;
            set => _zoomPoint = value;
        }

        public float PercentW(float percent) => width * (percent / 100f);

        public float PercentH(float percent) => height * (percent / 100f);

        public Vec2 PercentWH(float wide, float high) => new Vec2(width * (wide / 100f), height * (high / 100f));

        public Vec2 OffsetTL(float t, float l) => new Vec2(x + t, y + l);

        public Vec2 OffsetBR(float t, float l) => new Vec2(x + width + t, y + height + l);

        public Vec2 OffsetCenter(float t, float l) => new Vec2(x + PercentW(50f) + t, y + PercentH(50f) + l);

        public Camera()
        {
        }

        public Camera(float xval, float yval, float wval = -1f, float hval = -1f)
        {
            if (wval < 0f)
                wval = 320f;
            if (hval < 0f)
                hval = 320f * Graphics.aspect;
            x = xval;
            y = yval;
            width = wval;
            height = hval;
        }

        public void DoUpdate()
        {
            if (skipUpdate)
                skipUpdate = false;
            else
                Update();
        }
        public virtual void LerpCamera()
        {
            if (!LerpWithoutFear)
            {
                CameraLerp.CanLerp = false;
                CameraLerp.UpdateLerpState(_position, new Vec2(width, height), 1.0f, true);
            }
            else
            {
                CameraLerp.CanLerp = true;
                CameraLerp.UpdateLerpState(_position, new Vec2(width, height), MonoMain.IntraTick, MonoMain.UpdateLerpState);
            }
            _dirty = true;
        }
        public virtual void SubFrameUpdate()
        {
            if (LerpWithoutFear)
                return;
            CameraLerp.CanLerp = false;
            CameraLerp.SubFrameUpdate(_position, MonoMain.IntraTick);
            _dirty = true;
        }

        public virtual void Update()
        {
        }

        public virtual Vec2 transformScreenVector(Vec2 vector)
        {
            Vec3 vec3 = Vec3.Transform(new Vec3(vector.x, vector.y, 0f), Matrix.Invert(getMatrix()));
            return new Vec2(vec3.x, vec3.y);
        }

        public virtual Vec2 transformTime(Vec2 vector)
        {
            Vec3 vec3 = Vec3.Transform(new Vec3(vector.x, vector.y, 0f), Resolution.getTransformationMatrix() * getMatrix());
            return new Vec2(vec3.x, vec3.y);
        }

        public virtual Vec2 transformWorldVector(Vec2 vector)
        {
            Vec3 vec3 = Vec3.Transform(new Vec3(vector.x, vector.y, 0f), Matrix.Invert(Resolution.getTransformationMatrix()) * getMatrix());
            return new Vec2(vec3.x, vec3.y);
        }

        public virtual Vec2 transform(Vec2 vector)
        {
            Vec3 vec3 = Vec3.Transform(new Vec3(vector.x, vector.y, 0f), getMatrix());
            return new Vec2(vec3.x, vec3.y);
        }

        public virtual Vec2 transformInverse(Vec2 vector)
        {
            Vec3 vec3 = Vec3.Transform(new Vec3(vector.x, vector.y, 0f), Matrix.Invert(getMatrix()));
            return new Vec2(vec3.x, vec3.y);
        }

        public Rectangle rectangle => _rectangle;

        public virtual Matrix getMatrix()
        {
            Viewport viewport;
            if (!_dirty)
            {
                viewport = Graphics.viewport;
                if (viewport.Width == _viewSize.x)
                {
                    viewport = Graphics.viewport;
                    if (viewport.Height == _viewSize.y)
                        return _matrix;
                }
            }
            _rectangle = new Rectangle(left - 16f, top - 16f, size.x + 32f, size.y + 32f);
            viewport = Graphics.viewport;
            double width1 = viewport.Width;
            viewport = Graphics.viewport;
            double height1 = viewport.Height;
            _viewSize = new Vec2((float)width1, (float)height1);
            Vec2 position = DGRSettings.UncappedFPS ? CameraLerp.Position : _position;
            float width2 = CameraLerp.Size.x != 0f  && DGRSettings.UncappedFPS ? CameraLerp.Size.x : width;
            float height2 = CameraLerp.Size.y != 0f && DGRSettings.UncappedFPS ? CameraLerp.Size.y : height;
            _matrix = Matrix.CreateTranslation(new Vec3(-position.x, -position.y, 0f)) * Matrix.CreateScale(_viewSize.x / width2, _viewSize.y / height2, 1f);
            _dirty = false;
            return _matrix;
        }
    }
}
