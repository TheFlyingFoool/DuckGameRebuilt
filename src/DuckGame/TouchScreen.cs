// Decompiled with JetBrains decompiler
// Type: DuckGame.TouchScreen
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using System;
using System.Collections.Generic;
using System.Linq;

namespace DuckGame
{
    internal class TouchScreen
    {
        private static Dictionary<int, Touch> _touches = new Dictionary<int, Touch>();
        private static List<int> _removeTouches = new List<int>();
        private static ulong _totalFrameCount = 0;
        private static bool _updated = false;
        public static float _spoofFingerDistance = 0.0f;
        public static float _spoofFinger1Waver = 0.0f;
        public static float _spoofFinger2Waver = 0.0f;

        private static void System_MapTouch(TSData pTouch)
        {
            if (!TouchScreen._touches.ContainsKey(pTouch.fingerId))
                TouchScreen._touches[pTouch.fingerId] = new Touch()
                {
                    state = InputState.Pressed,
                    touchFrame = TouchScreen._totalFrameCount,
                    tap = true
                };
            else
                TouchScreen._touches[pTouch.fingerId].state = InputState.Down;
            TouchScreen._touches[pTouch.fingerId].SetData(pTouch);
            if (TouchScreen._touches.Count <= 1)
                return;
            foreach (KeyValuePair<int, Touch> touch in TouchScreen._touches)
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
            TouchScreen.System_TryUpdateIfNeeded();
            if (TouchScreen._touches.Count == 1)
            {
                Touch tap = TouchScreen._touches.First<KeyValuePair<int, Touch>>().Value;
                if (tap.state == InputState.Released && tap.tap && TouchScreen._totalFrameCount - tap.touchFrame < 20UL)
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
            TouchScreen.System_TryUpdateIfNeeded();
            if (TouchScreen._touches.Count == 1)
            {
                Touch press = TouchScreen._touches.First<KeyValuePair<int, Touch>>().Value;
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
            TouchScreen.System_TryUpdateIfNeeded();
            if (TouchScreen._touches.Count == 1)
            {
                Touch drag = TouchScreen._touches.First<KeyValuePair<int, Touch>>().Value;
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
            TouchScreen.System_TryUpdateIfNeeded();
            return TouchScreen._touches.Count == 1 && (TouchScreen._touches.First<KeyValuePair<int, Touch>>().Value.state == InputState.Pressed || TouchScreen._touches.First<KeyValuePair<int, Touch>>().Value.state == InputState.Down || TouchScreen._touches.First<KeyValuePair<int, Touch>>().Value.state == InputState.Released) ? TouchScreen._touches.First<KeyValuePair<int, Touch>>().Value : Touch.None;
        }

        /// <summary>
        /// GetRelease returns a touch if a single finger has just pulled itself from the screen.
        /// it also grants the TouchScreen class some sweet release, though this has no visible or actual effect
        /// </summary>
        /// <returns>The Touch in question</returns>
        public static Touch GetRelease()
        {
            TouchScreen.System_TryUpdateIfNeeded();
            return TouchScreen._touches.Count == 1 && TouchScreen._touches.First<KeyValuePair<int, Touch>>().Value.state == InputState.Released ? TouchScreen._touches.First<KeyValuePair<int, Touch>>().Value : Touch.None;
        }

        /// <summary>
        /// GetTouches returns a list of fingers currently on the screen
        /// </summary>
        /// <returns>Touches!</returns>
        public static List<Touch> GetTouches()
        {
            TouchScreen.System_TryUpdateIfNeeded();
            List<Touch> touches = new List<Touch>();
            foreach (KeyValuePair<int, Touch> touch in TouchScreen._touches)
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
            TouchScreen.System_TryUpdateIfNeeded();
            if (TouchScreen._touches.Count == 0)
                return Touch.None;
            Touch averageOfTouches = new Touch()
            {
                data = new TSData(0)
            };
            foreach (KeyValuePair<int, Touch> touch in TouchScreen._touches)
                averageOfTouches.data.touchXY += touch.Value.data.touchXY;
            averageOfTouches.data.touchXY /= (float)TouchScreen._touches.Count;
            return averageOfTouches;
        }

        public static void Update() => TouchScreen.System_DoUpdate(false);

        private static void System_TryUpdateIfNeeded()
        {
            if (TouchScreen._updated)
                return;
            TouchScreen.System_DoUpdate(true);
        }

        private static void System_DoUpdate(bool pForce)
        {
            if (TouchScreen._touches.Count > 0 | pForce)
            {
                TouchScreen._removeTouches.Clear();
                foreach (KeyValuePair<int, Touch> touch in TouchScreen._touches)
                {
                    if (touch.Value.state == InputState.Released)
                    {
                        TouchScreen._removeTouches.Add(touch.Key);
                        touch.Value.state = InputState.None;
                    }
                }
                foreach (int removeTouch in TouchScreen._removeTouches)
                    TouchScreen._touches.Remove(removeTouch);
                foreach (KeyValuePair<int, Touch> touch in TouchScreen._touches)
                    touch.Value.state = InputState.Released;
                if (Editor.fakeTouch)
                {
                    if (Mouse.left == InputState.Down)
                        TouchScreen.System_MapTouch(new TSData(0)
                        {
                            fingerId = 0,
                            touchXY = new Vec2(Mouse.xConsole - TouchScreen._spoofFingerDistance, Mouse.yConsole) + new Vec2((float)Math.Sin((double)TouchScreen._spoofFinger1Waver), (float)Math.Cos((double)TouchScreen._spoofFinger1Waver * 2.0)) * 2f
                        });
                    if (Mouse.right == InputState.Down)
                        TouchScreen.System_MapTouch(new TSData(0)
                        {
                            fingerId = 1,
                            touchXY = new Vec2(Mouse.xConsole + TouchScreen._spoofFingerDistance, Mouse.yConsole) + new Vec2((float)Math.Sin((double)TouchScreen._spoofFinger2Waver * 1.5), (float)Math.Cos((double)TouchScreen._spoofFinger2Waver * 0.300000011920929)) * 3f
                        });
                    if (Mouse.middle == InputState.Down)
                        TouchScreen.System_MapTouch(new TSData(0)
                        {
                            fingerId = 2,
                            touchXY = new Vec2(Mouse.xConsole, Mouse.yConsole)
                        });
                    TouchScreen._spoofFingerDistance += Mouse.scroll * 0.1f;
                    if ((double)TouchScreen._spoofFingerDistance < 0.0)
                        TouchScreen._spoofFingerDistance = 0.0f;
                }
                TouchScreen._updated = true;
            }
            else
                TouchScreen._updated = false;
            ++TouchScreen._totalFrameCount;
        }

        public static bool IsScreenTouched()
        {
            TouchScreen.System_TryUpdateIfNeeded();
            return TouchScreen._touches.Count > 0;
        }

        public static bool IsTouchScreenActive() => false;

        private static Vec2 System_FastTransformTouchScreenToCustomCamera(
          Vec2 touchXY,
          Camera customCam)
        {
            Vec2 customCamera = new Vec2();
            float num1 = touchXY.x / (float)Graphics.viewport.Width;
            float num2 = touchXY.y / (float)Graphics.viewport.Height;
            customCamera.x = num1 * customCam.width;
            customCamera.y = num2 * customCam.height;
            return customCamera;
        }
    }
}
