// Decompiled with JetBrains decompiler
// Type: DuckGame.TouchScreen
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    internal static class TouchScreen
    {
        private static Dictionary<int, Touch> _touches = new Dictionary<int, Touch>();
        private static List<int> _removeTouches = new List<int>();
        private static ulong _totalFrameCount = 0;
        private static bool _updated = false;
        public static float _spoofFingerDistance = 0f;
        public static float _spoofFinger1Waver = 0f;
        public static float _spoofFinger2Waver = 0f;

        private static void System_MapTouch(TSData pTouch)
        {
            if (!_touches.ContainsKey(pTouch.fingerId))
                _touches[pTouch.fingerId] = new Touch()
                {
                    state = InputState.Pressed,
                    touchFrame = _totalFrameCount,
                    tap = true
                };
            else
                _touches[pTouch.fingerId].state = InputState.Down;
            _touches[pTouch.fingerId].SetData(pTouch);
            if (_touches.Count <= 1)
                return;
            foreach (KeyValuePair<int, Touch> touch in _touches)
            {
                touch.Value.tap = false;
                touch.Value.canBeDrag = false;
            }
        }

        /// <summary>
        /// GetTap returns a touch if a single finger has just touched and released a place on the screen
        /// </summary>
        /// <returns>The Touch in question</returns>
        public static Touch GetTap()
        {
            System_TryUpdateIfNeeded();
            if (_touches.Count == 1)
            {
                Touch tap = _touches.First().Value;
                if (tap.state == InputState.Released && tap.tap && _totalFrameCount - tap.touchFrame < 20UL)
                    return tap;
            }
            return Touch.None;
        }

        /// <summary>
        /// GetPress returns a touch if a single finger has just touched a place on the screen
        /// </summary>
        /// <returns>The Touch in question</returns>
        public static Touch GetPress()
        {
            System_TryUpdateIfNeeded();
            if (_touches.Count == 1)
            {
                Touch press = _touches.First().Value;
                if (press.state == InputState.Pressed)
                    return press;
            }
            return Touch.None;
        }

        /// <summary>
        /// GetDrag returns a touch if a single finger is dragging along the screen
        /// </summary>
        /// <returns>The Touch in question</returns>
        public static Touch GetDrag()
        {
            System_TryUpdateIfNeeded();
            if (_touches.Count == 1)
            {
                Touch drag = _touches.First().Value;
                if (drag.state == InputState.Down && drag.drag)
                    return drag;
            }
            return Touch.None;
        }

        /// <summary>
        /// GetTouch returns a touch if a single finger is currently placed on the screen
        /// </summary>
        /// <returns>The Touch in question</returns>
        public static Touch GetTouch()
        {
            System_TryUpdateIfNeeded();
            return _touches.Count == 1 && (_touches.First().Value.state == InputState.Pressed || _touches.First().Value.state == InputState.Down || _touches.First().Value.state == InputState.Released) ? _touches.First().Value : Touch.None;
        }

        /// <summary>
        /// GetRelease returns a touch if a single finger has just pulled itself from the screen.
        /// it also grants the TouchScreen class some sweet release, though this has no visible or actual effect
        /// </summary>
        /// <returns>The Touch in question</returns>
        public static Touch GetRelease()
        {
            System_TryUpdateIfNeeded();
            return _touches.Count == 1 && _touches.First().Value.state == InputState.Released ? _touches.First().Value : Touch.None;
        }

        /// <summary>
        /// GetTouches returns a list of fingers currently on the screen
        /// </summary>
        /// <returns>Touches!</returns>
        public static List<Touch> GetTouches()
        {
            System_TryUpdateIfNeeded();
            List<Touch> touches = new List<Touch>();
            foreach (KeyValuePair<int, Touch> touch in _touches)
            {
                if (touch.Value.state != InputState.None)
                    touches.Add(touch.Value);
            }
            return touches;
        }

        /// <summary>
        /// Gets a touch that represents the average position of all touches
        /// </summary>
        /// <returns></returns>
        public static Touch GetAverageOfTouches()
        {
            System_TryUpdateIfNeeded();
            if (_touches.Count == 0)
                return Touch.None;
            Touch averageOfTouches = new Touch()
            {
                data = new TSData(0)
            };
            foreach (KeyValuePair<int, Touch> touch in _touches)
                averageOfTouches.data.touchXY += touch.Value.data.touchXY;
            averageOfTouches.data.touchXY /= _touches.Count;
            return averageOfTouches;
        }

        public static void Update() => System_DoUpdate(false);

        private static void System_TryUpdateIfNeeded()
        {
            if (_updated)
                return;
            System_DoUpdate(true);
        }

        private static void System_DoUpdate(bool pForce)
        {
            if (_touches.Count > 0 | pForce)
            {
                _removeTouches.Clear();
                foreach (KeyValuePair<int, Touch> touch in _touches)
                {
                    if (touch.Value.state == InputState.Released)
                    {
                        _removeTouches.Add(touch.Key);
                        touch.Value.state = InputState.None;
                    }
                }
                foreach (int removeTouch in _removeTouches)
                    _touches.Remove(removeTouch);
                foreach (KeyValuePair<int, Touch> touch in _touches)
                    touch.Value.state = InputState.Released;
                if (Editor.fakeTouch)
                {
                    if (Mouse.left == InputState.Down)
                        System_MapTouch(new TSData(0)
                        {
                            fingerId = 0,
                            touchXY = new Vec2(Mouse.xConsole - _spoofFingerDistance, Mouse.yConsole) + new Vec2((float)Math.Sin(_spoofFinger1Waver), (float)Math.Cos(_spoofFinger1Waver * 2.0)) * 2f
                        });
                    if (Mouse.right == InputState.Down)
                        System_MapTouch(new TSData(0)
                        {
                            fingerId = 1,
                            touchXY = new Vec2(Mouse.xConsole + _spoofFingerDistance, Mouse.yConsole) + new Vec2((float)Math.Sin((double)(_spoofFinger2Waver * 1.5f)), (float)Math.Cos((double)(_spoofFinger2Waver * 0.3f))) * 3f

                        });
                    if (Mouse.middle == InputState.Down)
                        System_MapTouch(new TSData(0)
                        {
                            fingerId = 2,
                            touchXY = new Vec2(Mouse.xConsole, Mouse.yConsole)
                        });
                    _spoofFingerDistance += Mouse.scroll * 0.1f;
                    if (_spoofFingerDistance < 0.0)
                        _spoofFingerDistance = 0f;
                }
                _updated = true;
            }
            else
                _updated = false;
            ++_totalFrameCount;
        }

        public static bool IsScreenTouched()
        {
            System_TryUpdateIfNeeded();
            return _touches.Count > 0;
        }

        public static bool IsTouchScreenActive() => false;

        //private static Vec2 System_FastTransformTouchScreenToCustomCamera(
        //  Vec2 touchXY,
        //  Camera customCam)
        //{
        //    Vec2 customCamera = new Vec2();
        //    float num1 = touchXY.x / Graphics.viewport.Width;
        //    float num2 = touchXY.y / Graphics.viewport.Height;
        //    customCamera.x = num1 * customCam.width;
        //    customCamera.y = num2 * customCam.height;
        //    return customCamera;
        //}
    }
}
