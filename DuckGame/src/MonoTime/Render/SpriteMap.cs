// Decompiled with JetBrains decompiler
// Type: DuckGame.SpriteMap
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace DuckGame
{
    public class SpriteMap : Sprite, ICloneable<SpriteMap>, ICloneable
    {
        private int _globalIndex = Thing.GetGlobalIndex();
        private int _width;
        private int _height;

        public bool canRegress;
        public bool canMultiframeSkip;

        public float _speed = 1f;
        private bool _finished;
        private List<Animation> _animations = new List<Animation>();
        private Animation? _currentAnimation;
        private bool _hasAnimation;
        public int _frame;
        private int _lastFrame = -1;
        public int _imageIndex;
        private int _lastImageIndex = -1;
        private Rectangle _spriteBox;
        public float _frameInc;
        private static Dictionary<string, List<bool>> _transparency = new Dictionary<string, List<bool>>();
        private int _cutWidth;
        private bool _flipFlop = true;
        private MTSpriteBatchItem _batchItem;
        private int _waitFrames;
        public int frames;
        public new int globalIndex
        {
            get => _globalIndex;
            set => _globalIndex = value;
        }

        public override int width => _width;

        public override int height => _height;

        public float speed
        {
            get => _speed;
            set => _speed = value;
        }

        public bool finished
        {
            get => _finished;
            set => _finished = value;
        }

        public int frame
        {
            get => _frame;
            set
            {
                SetFrameWithoutReset(value);
                _frameInc = 0f;
                _finished = false;
            }
        }

        public int imageIndex
        {
            get => _imageIndex;
            set => _imageIndex = value;
        }

        public int animationIndex
        {
            get => _currentAnimation.HasValue && _currentAnimation.HasValue && _animations.Contains(_currentAnimation.Value) ? _animations.IndexOf(_currentAnimation.Value) : 0;
            set
            {
                if (_animations == null)
                    return;
                SetAnimation(_animations[value].name);
            }
        }

        private bool valid => _texture != null && _texture.w > 0 && w > 0;

        public void SetFrameWithoutReset(int frame)
        {
            _frame = frame;
            if (_currentAnimation.HasValue)//&& valid)
            {
                if (_frame >= _currentAnimation.Value.frames.Length)
                    _frame = _currentAnimation.Value.frames.Length - 1;
                if (_frame < 0)
                    _frame = 0;
                _imageIndex = _currentAnimation.Value.frames[_frame];
            }
            else
                _imageIndex = _frame;
        }

        public string currentAnimation
        {
            get => !_currentAnimation.HasValue ? "" : _currentAnimation.Value.name;
            set => SetAnimation(value);
        }

        public SpriteMap(Tex2D tex, int frameWidth, int frameHeight)
        {
            _texture = tex;
            frameWidth = Math.Min(_texture.width, frameWidth);
            frameHeight = Math.Min(_texture.height, frameHeight);
            tex.frameWidth = frameWidth;
            tex.frameHeight = frameHeight;
            position = new Vec2(x, y);
            _width = frameWidth;
            _height = frameHeight;
            AddDefaultAnimation();
        }

        public SpriteMap(string tex, int frameWidth, int frameHeight, int pFrame)
          : this(tex, frameWidth, frameHeight)
        {
            frame = pFrame;
        }

        public SpriteMap(string tex, int frameWidth, int frameHeight, bool calculateTransparency = false)
        {
            _texture = Content.Load<Tex2D>(tex);
            frameWidth = Math.Min(_texture.width, frameWidth);
            frameHeight = Math.Min(_texture.height, frameHeight);
            _texture.frameWidth = frameWidth;
            _texture.frameHeight = frameHeight;
            position = new Vec2(x, y);
            _width = frameWidth;
            _height = frameHeight;
            AddDefaultAnimation();
            int num = calculateTransparency ? 1 : 0;
        }

        public bool CurrentFrameIsOpaque() => false;

        private void AddDefaultAnimation()
        {
            int length = 1;
            if (_width > 0)
                length = _texture.width / _width * (_texture.height / _height);
            int[] framesVal = new int[length];
            for (int index = 0; index < length; ++index)
                framesVal[index] = index;
            _animations.Add(new Animation("default", 1f, true, framesVal));
            SetAnimation("default");
            _speed = 0f;
        }

        public void AddAnimation(string name, float speed, bool looping, params int[] frames)
        {
            if (!_hasAnimation)
            {
                ClearAnimations();
                _speed = 1f;
            }
            _hasAnimation = true;
            _animations.Add(new Animation(name, speed, looping, frames));
        }

        public void SetAnimation(string name)
        {
            if (_currentAnimation.HasValue && _currentAnimation.Value.name == name)
                return;
            _finished = false;
            foreach (Animation animation in _animations)
            {
                if (animation.name == name)
                {
                    _currentAnimation = new Animation?(animation);
                    frames = _currentAnimation.Value.frames.Length;
                    _frameInc = 0f;
                    frame = 0;
                    return;
                }
            }
            _currentAnimation = new Animation?();
        }

        public void ClearAnimations()
        {
            _animations.Clear();
            _currentAnimation = new Animation?();
        }

        public void CloneAnimations(SpriteMap into) => into._animations = new List<Animation>(_animations);

        public void UpdateSpriteBox()
        {
            if (!valid)
                return;
            int num1 = _texture.width / w;
            int num2 = _imageIndex / num1;
            _spriteBox = new Rectangle((_imageIndex - num2 * num1) * w, num2 * h, w - cutWidth, h);
            _lastImageIndex = _imageIndex;
        }

        public bool UpdateFrame(bool ignoreFlipFlop = false)
        {
            if (!valid)
                return false;
            if (_currentAnimation.HasValue && (ignoreFlipFlop || _flipFlop != Graphics.frameFlipFlop) && !VirtualTransition.doingVirtualTransition)
            {
                _frameInc += _currentAnimation.Value.speed * _speed;
                if (_frameInc >= 1)
                {
                    if (canMultiframeSkip)
                    {
                        _frameInc--;
                        if (_frameInc <= -1)
                        {
                            _frameInc--;
                            ++_frame;
                        }
                    }
                    else _frameInc = 0;
                    ++_frame;
                }
                else if (canRegress && _frameInc <= -1)
                {
                    if (canMultiframeSkip)
                    {
                        _frameInc++;
                        if (_frameInc <= -1)
                        {
                            _frameInc++;
                            --_frame;
                        }
                    }
                    else _frameInc = 0;
                    --_frame;
                }
                if (_lastFrame != _frame)
                {
                    if (_frame >= _currentAnimation.Value.frames.Length)
                    {
                        if (_currentAnimation.Value.looping)
                        {
                            frame = 0;
                        }
                        else
                        {
                            frame = _currentAnimation.Value.frames.Length - 1;
                            finished = true;
                        }
                    }
                    else if (canRegress && _frame < 0)
                    {
                        if (_currentAnimation.Value.looping)
                        {
                            frame = _currentAnimation.Value.frames.Length - 1;
                        }
                        else
                        {
                            frame = 0;
                            finished = true;
                        }
                    }
                    _imageIndex = _currentAnimation.Value.frames[_frame];
                    _lastFrame = _frame;
                }
                _flipFlop = !_flipFlop;
            }
            if (_lastImageIndex != _imageIndex)
                UpdateSpriteBox();
            return true;
        }

        public void UpdateFrameSpecial()
        {
            if (!valid)
                return;
            if (_currentAnimation.HasValue && !VirtualTransition.doingVirtualTransition)
            {
                _frameInc += _currentAnimation.Value.speed * _speed;
                if (_frameInc >= 1)
                {
                    _frameInc = 0f;
                    ++_frame;
                }
                if (_frame >= _currentAnimation.Value.frames.Length)
                {
                    if (_currentAnimation.Value.looping)
                    {
                        frame = 0;
                    }
                    else
                    {
                        frame = _currentAnimation.Value.frames.Length - 1;
                        finished = true;
                    }
                }
                _imageIndex = _currentAnimation.Value.frames[_frame];
            }
            UpdateSpriteBox();
        }

        public int cutWidth
        {
            get => _cutWidth;
            set
            {
                _cutWidth = value;
                UpdateSpriteBox();
            }
        }

        public override void Draw()
        {
            if (!UpdateFrame())
                return;
            _texture.currentObjectIndex = _globalIndex;
            if (w <= 0)
                return;
            Graphics.Draw(_texture, position, new Rectangle?(_spriteBox), _color * alpha, angle, center, scale, flipH ? SpriteEffects.FlipHorizontally : (flipV ? SpriteEffects.FlipVertically : SpriteEffects.None), depth);
        }

        public override void Draw(Rectangle r)
        {
            if (!UpdateFrame())
                return;
            r.x += _spriteBox.x;
            r.y += _spriteBox.y;
            _texture.currentObjectIndex = _globalIndex;
            Graphics.Draw(_texture, position, new Rectangle?(r), _color * alpha, angle, center, scale, _flipH ? SpriteEffects.FlipHorizontally : (_flipV ? SpriteEffects.FlipVertically : SpriteEffects.None), depth);
        }

        public void DrawWithoutUpdate()
        {
            if (!valid)
                return;
            _texture.currentObjectIndex = _globalIndex;
            if (w <= 0)
                return;
            Graphics.Draw(_texture, position, new Rectangle?(_spriteBox), _color * alpha, angle, center, scale, flipH ? SpriteEffects.FlipHorizontally : (flipV ? SpriteEffects.FlipVertically : SpriteEffects.None), depth);
        }

        public override void CheapDraw(bool flipH = false)
        {
            if (!valid)
                return;
            _texture.currentObjectIndex = _globalIndex;
            Graphics.Draw(_texture, position, new Rectangle?(_spriteBox), _color, angle, center, scale, flipH ? SpriteEffects.FlipHorizontally : SpriteEffects.None, depth);
        }

        public void ClearCache()
        {
            _batchItem = null;
            _waitFrames = 0;
        }

        public override void UltraCheapStaticDraw(bool flipH = false)
        {
            if (_batchItem == null)
            {
                if (!valid)
                    return;
                UpdateFrame();
                Graphics.recordMetadata = true;
                _texture.currentObjectIndex = _globalIndex;
                Graphics.Draw(_texture, position, new Rectangle?(_spriteBox), _color, angle, center, scale, flipH ? SpriteEffects.FlipHorizontally : SpriteEffects.None, _depth);
                if (_waitFrames == 1)
                {
                    _batchItem = Graphics.screen.StealLastSpriteBatchItem();
                    if (_batchItem.MetaData == null)
                        _batchItem = null;
                }
                ++_waitFrames;
                Graphics.recordMetadata = false;
            }
            else
            {
                _batchItem.Material = cheapmaterial;
                _texture.currentObjectIndex = _globalIndex;
                Graphics.Draw(_batchItem);
            }
        }

        public override Sprite Clone()
        {
            SpriteMap into = new SpriteMap(_texture, _width, _height);
            CloneAnimations(into);
            into.center = center;
            into.imageIndex = imageIndex;
            into.frame = frame;
            into._globalIndex = _globalIndex;
            return into;
        }

        public SpriteMap CloneMap() => (SpriteMap)Clone();

        SpriteMap ICloneable<SpriteMap>.Clone() => (SpriteMap)Clone();

        object ICloneable.Clone() => Clone();
    }
}
