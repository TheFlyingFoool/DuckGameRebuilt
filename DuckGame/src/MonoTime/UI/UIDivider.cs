// Decompiled with JetBrains decompiler
// Type: DuckGame.UIDivider
//removed for regex reasons Culture=neutral, PublicKeyToken=null
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

        public UIBox leftSection => _leftBox;

        public UIBox rightSection => _rightBox;

        public UIBox topSection => _leftBox;

        public UIBox bottomSection => _rightBox;
        private Interp LeftLerp = new Interp(true);
        private Interp RightLerp = new Interp(true);

        public UIDivider(bool vert, float splitVal, float sep = 1f)
          : base(0f, 0f, 0f, 0f)
        {
            _vertical = vert;
            _splitPercent = splitVal;
            _leftBox = new UIBox(isVisible: false);
            _rightBox = new UIBox(isVisible: false);
            Add(_leftBox);
            Add(_rightBox);
            _canFit = true;
            _seperation = sep;
        }

        public UIDivider(bool vert, int splitVal, float sep = 1f)
          : base(0f, 0f, 0f, 0f)
        {
            _vertical = vert;
            _leftBox = new UIBox(isVisible: false);
            _rightBox = new UIBox(isVisible: false);
            Add(_leftBox);
            Add(_rightBox);
            _splitPixels = splitVal;
            _canFit = true;
            _seperation = sep;
        }

        protected override void SizeChildren()
        {
            if (_vertical)
            {
                Vec2 sizes = CalculateSizes();
                _leftBox.collisionSize = new Vec2(sizes.x, collisionSize.y - borderSize.y * 2f);
                _rightBox.collisionSize = new Vec2(sizes.y, collisionSize.y - borderSize.y * 2f);
            }
            else
            {
                Vec2 sizes = CalculateSizes();
                _leftBox.collisionSize = new Vec2(collisionSize.x - borderSize.x * 2f, sizes.x);
                _rightBox.collisionSize = new Vec2(collisionSize.x - borderSize.x * 2f, sizes.y);
            }
        }

        public Vec2 CalculateSizes()
        {
            Vec2 collisionSize = this.collisionSize;
            if (_vertical)
            {
                collisionSize.x -= _seperation;
                float x = _leftBox.collisionSize.x;
                float y = _rightBox.collisionSize.x;
                if (_splitPercent != 0.0)
                {
                    x = collisionSize.x * _splitPercent;
                    y = collisionSize.x * (1f - _splitPercent);
                }
                else if (_splitPixels > 0)
                {
                    x = _splitPixels;
                    y = collisionSize.x - _splitPixels;
                }
                return new Vec2(x, y);
            }
            collisionSize.y -= _seperation;
            float x1 = _leftBox.collisionSize.y;
            float y1 = _rightBox.collisionSize.y;
            if (_splitPercent != 0.0)
            {
                x1 = collisionSize.y * _splitPercent;
                y1 = collisionSize.y * (1f - _splitPercent);
            }
            else if (_splitPixels > 0)
            {
                x1 = _splitPixels;
                y1 = collisionSize.y - _splitPixels;
            }
            return new Vec2(x1, y1);
        }

        protected override void OnResize()
        {
            if (_vertical)
            {
                _collisionSize.y = Math.Max(_leftBox.collisionSize.y, _rightBox.collisionSize.y);
                float num1 = _leftBox.collisionSize.x + _rightBox.collisionSize.x + _seperation;
                if (_collisionSize.x < num1)
                    _collisionSize.x = num1;
                Vec2 sizes = CalculateSizes();
                float num2 = sizes.x;
                float num3 = sizes.y;
                if (num2 < _leftBox.collisionSize.x)
                    num2 = _leftBox.collisionSize.x;
                if (num3 < _rightBox.collisionSize.x)
                {
                    num3 = _rightBox.collisionSize.x;
                    num2 = _collisionSize.x - num3;
                }
                float num4 = Math.Max(_leftBox.collisionSize.y, _rightBox.collisionSize.y);
                if (_collisionSize.y < num4)
                    _collisionSize.y = num4;
                _leftBox.anchor.offset.x = (float)(-halfWidth + num2 / 2.0);
                _leftBox.anchor.offset.y = 0f;
                _rightBox.anchor.offset.x = halfWidth - num3 / 2f;
                _rightBox.anchor.offset.y = 0f;
            }
            else
            {
                _collisionSize.y = Math.Max(_leftBox.collisionSize.y, _splitPixels) + _rightBox.collisionSize.y;
                float num5 = _leftBox.collisionSize.y + _rightBox.collisionSize.y + _seperation;
                if (_collisionSize.y < num5)
                    _collisionSize.y = num5;
                Vec2 sizes = CalculateSizes();
                float num6 = sizes.x;
                float y = sizes.y;
                if (num6 < _leftBox.collisionSize.y)
                    num6 = _leftBox.collisionSize.y;
                if (y < _rightBox.collisionSize.y)
                {
                    y = _rightBox.collisionSize.y;
                    num6 = _collisionSize.y - y;
                }
                float num7 = Math.Max(_leftBox.collisionSize.x, _rightBox.collisionSize.x);
                if (_collisionSize.x < num7)
                    _collisionSize.x = num7;
                _leftBox.anchor.offset.x = 0f;
                _leftBox.anchor.offset.y = (float)(-halfHeight + num6 / 2.0);
                _rightBox.anchor.offset.x = 0f;
                _rightBox.anchor.offset.y = halfHeight - y / 2f;
            }
        }

        public override void Draw()
        {

            if (!_vertical)
            {
                Vec2 vec2_1 = _rightBox.position - new Vec2(_rightBox.width / 2f, _rightBox.height / 2f);
                Vec2 vec2_2 = _leftBox.position - new Vec2(_leftBox.width / 2f, _leftBox.height / 2f);
                if (vec2_2.x < vec2_1.x)
                    vec2_1.x = vec2_2.x;
                vec2_1.y -= _seperation / 2f;
                Vec2 vec2_3 = _rightBox.position + new Vec2(_rightBox.width / 2f, _rightBox.height / 2f);
                Vec2 vec2_4 = _leftBox.position + new Vec2(_leftBox.width / 2f, _leftBox.height / 2f);
                if (vec2_4.x > vec2_3.x)
                    vec2_3.x = vec2_4.x;
                if (_splitPixels == 0)
                {
                }
                else
                {
                }
                LeftLerp.UpdateLerpState(vec2_1, MonoMain.IntraTick, MonoMain.UpdateLerpState);
                RightLerp.UpdateLerpState(vec2_3, MonoMain.IntraTick, MonoMain.UpdateLerpState);

                Graphics.DrawLine(new Vec2(LeftLerp.x, LeftLerp.y), new Vec2(RightLerp.x, LeftLerp.y), Color.White, depth: (depth + 10));
            }
            int num = debug ? 1 : 0;
            base.Draw();
        }
    }
}
