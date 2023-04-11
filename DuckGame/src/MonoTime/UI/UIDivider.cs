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
            _splitPixels = splitVal;
            _leftBox = new UIBox(isVisible: false);
            _rightBox = new UIBox(isVisible: false);
            Add(_leftBox);
            Add(_rightBox);
            _canFit = true;
            _seperation = sep;
        }

        protected override void SizeChildren()
        {
            Vec2 sizes = CalculateSizes();
            if (_vertical)
            {
                _leftBox.collisionSize = new Vec2(sizes.x, collisionSize.y - borderSize.y * 2f);
                _rightBox.collisionSize = new Vec2(sizes.y, collisionSize.y - borderSize.y * 2f);
            }
            else
            {
                _leftBox.collisionSize = new Vec2(collisionSize.x - borderSize.x * 2f, sizes.x);
                _rightBox.collisionSize = new Vec2(collisionSize.x - borderSize.x * 2f, sizes.y);
            }
        }

        public Vec2 CalculateSizes()
        {
            Vec2 colSize = this.collisionSize;
            if (_vertical)
            {
                colSize.x -= _seperation;
                float leftSize = _leftBox.collisionSize.x;
                float rightSize = _rightBox.collisionSize.x;
                if (_splitPercent != 0.0)
                {
                    leftSize = colSize.x * _splitPercent;
                    rightSize = colSize.x * (1f - _splitPercent);
                }
                else if (_splitPixels > 0)
                {
                    leftSize = _splitPixels;
                    rightSize = colSize.x - _splitPixels;
                }
                return new Vec2(leftSize, rightSize);
            }
            colSize.y -= _seperation;
            float topSize = _leftBox.collisionSize.y;
            float bottomSize = _rightBox.collisionSize.y;
            if (_splitPercent != 0.0)
            {
                topSize = colSize.y * _splitPercent;
                bottomSize = colSize.y * (1f - _splitPercent);
            }
            else if (_splitPixels > 0)
            {
                topSize = _splitPixels;
                bottomSize = colSize.y - _splitPixels;
            }
            return new Vec2(topSize, bottomSize);
        }

        protected override void OnResize()
        {
            if (_vertical)
            {
                _collisionSize.y = Math.Max(_leftBox.collisionSize.y, _rightBox.collisionSize.y);
                float minWidth = _leftBox.collisionSize.x + _rightBox.collisionSize.x + _seperation;
                if (_collisionSize.x < minWidth)
                    _collisionSize.x = minWidth;
                Vec2 sizes = CalculateSizes();
                float leftSize = sizes.x;
                float rightSize = sizes.y;
                if (leftSize < _leftBox.collisionSize.x)
                    leftSize = _leftBox.collisionSize.x;
                if (rightSize < _rightBox.collisionSize.x)
                {
                    rightSize = _rightBox.collisionSize.x;
                    leftSize = _collisionSize.x - rightSize;
                }
                float minHeight = Math.Max(_leftBox.collisionSize.y, _rightBox.collisionSize.y);
                if (_collisionSize.y < minHeight)
                    _collisionSize.y = minHeight;
                _leftBox.anchor.offset.x = -halfWidth + leftSize / 2f;
                _leftBox.anchor.offset.y = 0f;
                _rightBox.anchor.offset.x = halfWidth - rightSize / 2f;
                _rightBox.anchor.offset.y = 0f;
            }
            else
            {
                _collisionSize.y = Math.Max(_leftBox.collisionSize.y, _splitPixels) + _rightBox.collisionSize.y;
                float minHeight2 = _leftBox.collisionSize.y + _rightBox.collisionSize.y + _seperation;
                if (_collisionSize.y < minHeight2)
                    _collisionSize.y = minHeight2;
                Vec2 sizes = CalculateSizes();
                float topSize = sizes.x;
                float bottomSize = sizes.y;
                if (topSize < _leftBox.collisionSize.y)
                    topSize = _leftBox.collisionSize.y;
                if (bottomSize < _rightBox.collisionSize.y)
                {
                    bottomSize = _rightBox.collisionSize.y;
                    topSize = _collisionSize.y - bottomSize;
                }
                float minWidth2 = Math.Max(_leftBox.collisionSize.x, _rightBox.collisionSize.x);
                if (_collisionSize.x < minWidth2)
                    _collisionSize.x = minWidth2;
                _leftBox.anchor.offset.x = 0f;
                _leftBox.anchor.offset.y = -halfHeight + topSize / 2f;
                _rightBox.anchor.offset.x = 0f;
                _rightBox.anchor.offset.y = halfHeight - bottomSize / 2f;
            }
        }

        public override void Draw()
        {
            if (!_vertical)
            {
                Vec2 tl1 = _rightBox.position - new Vec2(_rightBox.width / 2f, _rightBox.height / 2f);
                Vec2 tl2 = _leftBox.position - new Vec2(_leftBox.width / 2f, _leftBox.height / 2f);
                if (tl2.x < tl1.x)
                    tl1.x = tl2.x;
                tl1.y -= _seperation / 2f;
                Vec2 br1 = _rightBox.position + new Vec2(_rightBox.width / 2f, _rightBox.height / 2f);
                Vec2 br2 = _leftBox.position + new Vec2(_leftBox.width / 2f, _leftBox.height / 2f);
                if (br2.x > br1.x)
                    br1.x = br2.x;
                Graphics.DrawLine(new Vec2(tl1.x, tl1.y), new Vec2(br1.x, tl1.y), Color.White, depth: (depth + 10));
            }
            base.Draw();
        }
    }
}
