// Decompiled with JetBrains decompiler
// Type: DuckGame.FollowCam
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    public class FollowCam : Camera
    {
        public Viewport storedViewport;
        private HashSet<Thing> _follow = new HashSet<Thing>();
        private Dictionary<Thing, Vec2> _prevPositions = new Dictionary<Thing, Vec2>();
        private float _viewSize;
        public float manualViewSize = -1f;
        public Vec2 _center;
        private float _lerpSpeed = 1f;
        private float _lerpBorder;
        private float border = 80f;
        public float minSize = 60f;
        private float _prevCenterMoveX;
        private float _prevCenterMoveY;
        private float _prevViewSize;
        private bool _startedFollowing;
        //private Vec2 _averagePosition = Vec2.Zero;
        private bool _startCentered = true;
        private float _lerpMult = 1f;
        private float _speed = 1f;
        private bool immediate;
        private bool _allowWarps;
        private bool _checkedZoom;
        private float _zoomMult = 1f;
        private bool _skipResize;
        private bool _overFollow;
        public float hardLimitLeft = -999999f;
        public float hardLimitRight = 999999f;
        public float hardLimitTop = -999999f;
        public float hardLimitBottom = 999999f;
        private bool woteFrame;
        //private CameraBounds _bounds;
        private List<Thing> _removeList = new List<Thing>();
        private int _framesCreated;
        public static bool boost;

        public float viewSize
        {
            get => _viewSize;
            set => _viewSize = value;
        }

        public float lerpSpeed
        {
            get => _lerpSpeed;
            set => _lerpSpeed = value;
        }

        public bool startCentered
        {
            get => _startCentered;
            set => _startCentered = value;
        }

        public float lerpMult
        {
            get => _lerpMult;
            set => _lerpMult = value;
        }

        public float speed
        {
            get => _speed;
            set => _speed = value;
        }

        public void Add(Thing t)
        {
            if (t == null)
                return;
            _follow.Add(t);
            _startedFollowing = true;
        }

        public void Clear()
        {
            _follow.Clear();
            _prevPositions.Clear();
        }

        public void Remove(Thing t)
        {
            _follow.Remove(t);
            if (t == null || !_prevPositions.ContainsKey(t))
                return;
            _prevPositions.Remove(t);
        }

        public bool Contains(Thing t) => _follow.Contains(t);

        public void Adjust()
        {
            Level.current.CalculateBounds();
            Update();
            immediate = true;
            Update();
            immediate = false;
        }

        public float zoomMult
        {
            get => _zoomMult;
            set => _zoomMult = value;
        }

        public override void Update()
        {
            ++_framesCreated;
            if (Network.isActive && (_framesCreated > 60 || _framesCreated > 120) && !_startedFollowing && _follow.Count == 0)
                _startedFollowing = false;
            if (Level.current is TeamSelect2)
            {
                _follow.RemoveWhere(x => x is Duck);
                foreach (Thing t in Level.current.things[typeof(Duck)])
                    Add(t);
            }
            if (!_checkedZoom)
            {
                _checkedZoom = true;
                CameraZoom cameraZoom = Level.First<CameraZoom>();
                if (cameraZoom != null)
                {
                    _zoomMult = cameraZoom.zoomMult;
                    _overFollow = (bool)cameraZoom.overFollow;
                    _allowWarps = (bool)cameraZoom.allowWarps;
                }
                if (Level.current is RandomLevel)
                    _zoomMult = 1.25f;
                CustomCamera customCamera = Level.First<CustomCamera>();
                if (customCamera != null)
                {
                    width = customCamera.wide.value;
                    height = customCamera.wide.value * (9f / 16f);
                    center = customCamera.position;
                    _skipResize = true;
                }
                if (Level.First<CameraBounds>() != null)
                    Adjust();
            }
            bool flag = true;
            if (_startedFollowing)
            {
                flag = false;
                foreach (Duck t in Level.current.things[typeof(Duck)])
                {
                    if (!t.dead)
                        flag = true;
                    if (Network.isActive && !t.dead)
                        Add(t);
                }
                if (Network.isActive)
                {
                    foreach (RCCar t in Level.current.things[typeof(RCCar)])
                    {
                        if (t.receivingSignal)
                            Add(t);
                        else
                            Remove(t);
                    }
                }
                foreach (Thing thing in _follow)
                {
                    if (thing is Duck duck && !duck.dead)
                        flag = true;
                }
                if (Network.isActive)
                {
                    for (int index = 0; index < _follow.Count; ++index)
                    {
                        if (!_follow.ElementAt<Thing>(index).active)
                        {
                            _follow.Remove(_follow.ElementAt<Thing>(index));
                            --index;
                        }
                    }
                }
            }
            if (_skipResize)
                return;
            float lerpMult = this.lerpMult;
            if (lerpSpeed > 0.9f)
                this.lerpMult = 1.8f;
            border += ((_lerpBorder - border) * (lerpSpeed * 16f * _lerpMult)) * _speed;
            if (immediate)
                border = _lerpBorder;
            float num1 = 99999f;
            float num2 = -99999f;
            float num3 = 99999f;
            float num4 = -99999f;
            Vec2 zero = Vec2.Zero;
            _removeList.Clear();
            foreach (Thing key in _follow)
            {
                Vec2 current = key.cameraPosition;
                if (key.removeFromLevel)
                    _removeList.Add(key);
                if (_prevPositions.ContainsKey(key))
                {
                    Vec2 prevPosition = _prevPositions[key];
                    if (_overFollow || FollowCam.boost || key.overfollow > 0f)
                    {
                        float amount = 0.3f;
                        if (key.overfollow > 0f)
                            amount = key.overfollow;
                        Vec2 vec2 = (key.cameraPosition - prevPosition) * 24f;
                        if (vec2.length > 100f)
                            vec2 = vec2.normalized * 100f;
                        Vec2 to = key.cameraPosition + vec2;
                        current = Lerp.Vec2Smooth(current, to, amount);
                    }
                    if ((prevPosition - current).length > 2500f && !_allowWarps)
                    {
                        current.x = prevPosition.x;
                        current.y = prevPosition.y;
                    }
                    else
                        _prevPositions[key] = key.cameraPosition;
                }
                else
                    _prevPositions[key] = key.cameraPosition;
                if (current.x < num1)
                    num1 = current.x;
                if (current.x > num2)
                    num2 = current.x;
                if (current.y < num3)
                    num3 = current.y;
                if (current.y > num4)
                    num4 = current.y;
                zero += current;
            }
            foreach (Thing remove in _removeList)
                Remove(remove);
            _removeList.Clear();
            float num5 = Level.current.topLeft.y - 64f;
            float y = Level.current.bottomRight.y;
            float x1 = Level.current.topLeft.x;
            float x2 = Level.current.bottomRight.x;
            if (num4 > y)
            {
                num4 = y;
                if (num3 > num4)
                    num3 = num4;
            }
            if (num3 < num5)
            {
                num3 = num5;
                if (num4 < num3)
                    num4 = num3;
            }
            if (num1 < x1)
            {
                num1 = x1;
                if (num2 < num1)
                    num2 = num1;
            }
            if (num2 > x2)
            {
                num2 = x2;
                if (num1 > num2)
                    num1 = num2;
            }
            float num6 = num3 - border;
            float num7 = num4 + border;
            float num8 = num1 - border;
            float num9 = num2 + border;
            float num10 = ((num8 + num9) / 2f);
            float num11 = ((num6 + num7) / 2f);
            float num12 = Resolution.current.x / (float)Resolution.current.y;
            float num13 = Math.Abs(num8 - num9);
            float num14 = Math.Abs(num6 - num7);
            if (lerpSpeed > 0.9f)
            {
                num13 = Level.current.bottomRight.x - Level.current.topLeft.x;
                num14 = Level.current.bottomRight.y - Level.current.topLeft.y;
            }
            float num15 = num14 <= num13 / num12 ? num13 / num12 : num14;
            if (!flag && woteFrame)
                num15 = _prevViewSize;
            else
                _prevViewSize = num15;
            _viewSize += ((num15 - _viewSize) * (lerpSpeed * _lerpMult * _speed));
            if (immediate)
                _viewSize = num15;
            float num16 = _viewSize;
            if (manualViewSize > 0.0)
                num16 = manualViewSize;
            width = num16 * num12;
            height = num16;
            _lerpBorder = Maths.Clamp((float)(Math.Min(width, 740f) / 740f * (90f * _zoomMult)), minSize * _zoomMult, 90f * _zoomMult);
            if (!flag && woteFrame)
            {
                num10 = _prevCenterMoveX;
                num11 = _prevCenterMoveY;
            }
            else
            {
                _prevCenterMoveX = num10;
                _prevCenterMoveY = num11;
            }
            if (!flag)
                woteFrame = true;
            _center.x += ((num10 - _center.x) * (lerpSpeed * _lerpMult));
            _center.y += ((num11 - _center.y) * (lerpSpeed * _lerpMult));
            if (immediate)
            {
                _center.x = num10;
                _center.y = num11;
            }
            if (lerpSpeed > 0.9f && _startCentered)
            {
                _center.x = ((Level.current.bottomRight.x + Level.current.topLeft.x) / 2f);
                _center.y = ((Level.current.bottomRight.y + Level.current.topLeft.y) / 2f);
            }
            x = _center.x - width / 2f;
            this.y = _center.y - height / 2f;
            if (x < hardLimitLeft)
                x = hardLimitLeft;
            if (right > hardLimitRight)
                x = hardLimitRight - width;
            if (this.y < hardLimitTop)
                this.y = hardLimitTop;
            if (bottom > hardLimitBottom)
                this.y = hardLimitBottom - height;
            if (_lerpSpeed > 0.9f)
                _lerpSpeed = 0.05f;
            this.lerpMult = lerpMult;
            FollowCam.boost = false;
        }
    }
}
