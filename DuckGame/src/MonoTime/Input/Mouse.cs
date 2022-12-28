// Decompiled with JetBrains decompiler
// Type: DuckGame.Mouse
//removed for regex reasons Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using Microsoft.Xna.Framework.Input;

namespace DuckGame
{
    public class Mouse : InputDevice
    {
        private static Vec2 _mousePos;
        private static Vec2 _mouseScreenPos;
        private static MouseState _mouseState;
        private static MouseState _mouseStatePrev;
        private static float _prevScrollValue;
        private static bool _outOfFocus;
        public const int kMouseLBMapping = 999990;
        public const int kMouseMBMapping = 999991;
        public const int kMouseRBMapping = 999992;
        public const int kMouseScrollUpMapping = 999993;
        public const int kMouseScrollDownMapping = 999994;

        public override void Update()
        {
            if (!Graphics.inFocus)
            {
                _outOfFocus = true;
            }
            else
            {
                _prevScrollValue = scroll;
                _mouseStatePrev = _mouseState;
                _mouseState = Microsoft.Xna.Framework.Input.Mouse.GetState();
                Vec3 vec3 = new Vec3(_mouseState.X, _mouseState.Y, 0f);
                if (Graphics._screenViewport.HasValue)
                    _mouseScreenPos = new Vec2(vec3.x / Resolution.size.x * Layer.HUD.camera.width, vec3.y / Resolution.size.y * Layer.HUD.camera.height);
                _mouseScreenPos.x = (int)_mouseScreenPos.x;
                _mouseScreenPos.y = (int)_mouseScreenPos.y;
                _mousePos = new Vec2(_mouseState.X, _mouseState.Y);
                if (!_outOfFocus)
                    return;
                if (_mouseState.LeftButton == ButtonState.Released && _mouseState.MiddleButton == ButtonState.Released && _mouseState.RightButton == ButtonState.Released)
                    _outOfFocus = false;
                else
                    _mouseState = _mouseStatePrev = new MouseState(_mouseState.X, _mouseState.Y, _mouseState.ScrollWheelValue, ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released);
            }
        }

        public static InputState left
        {
            get
            {
                if (_mouseState.LeftButton == ButtonState.Pressed && _mouseStatePrev.LeftButton == ButtonState.Released)
                    return InputState.Pressed;
                if (_mouseState.LeftButton == ButtonState.Pressed && _mouseStatePrev.LeftButton == ButtonState.Pressed)
                    return InputState.Down;
                return _mouseState.LeftButton == ButtonState.Released && _mouseStatePrev.LeftButton == ButtonState.Pressed ? InputState.Released : InputState.None;
            }
        }

        public static InputState middle
        {
            get
            {
                if (_mouseState.MiddleButton == ButtonState.Pressed && _mouseStatePrev.MiddleButton == ButtonState.Released)
                    return InputState.Pressed;
                if (_mouseState.MiddleButton == ButtonState.Pressed && _mouseStatePrev.MiddleButton == ButtonState.Pressed)
                    return InputState.Down;
                return _mouseState.MiddleButton == ButtonState.Released && _mouseStatePrev.MiddleButton == ButtonState.Pressed ? InputState.Released : InputState.None;
            }
        }

        public static InputState right
        {
            get
            {
                if (_mouseState.RightButton == ButtonState.Pressed && _mouseStatePrev.RightButton == ButtonState.Released)
                    return InputState.Pressed;
                if (_mouseState.RightButton == ButtonState.Pressed && _mouseStatePrev.RightButton == ButtonState.Pressed)
                    return InputState.Down;
                return _mouseState.RightButton == ButtonState.Released && _mouseStatePrev.RightButton == ButtonState.Pressed ? InputState.Released : InputState.None;
            }
        }

        public static bool available => true;

        public static float scroll => _mouseStatePrev.ScrollWheelValue - _mouseState.ScrollWheelValue;
        /// <summary>
        /// scrolling but min/maxed so you dont get values that change based on how much you're scrolling
        /// </summary>
        public static int discreteScroll => scrollingUp ? -1 : scrollingDown ? 1 : 0;

        public static bool isScrolling => scroll != 0;
        public static bool scrollingDown => scroll > 0;
        public static bool scrollingUp => scroll < 0;

        public static bool prevScrollDown => _prevScrollValue > 0.0;

        public static bool prevScrollUp => _prevScrollValue < 0.0;

        public static float x => _mouseScreenPos.x;

        public static float y => _mouseScreenPos.y;

        public static float xScreen => positionScreen.x;

        public static float yScreen => positionScreen.y;

        public static float xConsole => positionConsole.x;

        public static float yConsole => positionConsole.y;

        public static Vec2 position
        {
            get => new Vec2(x, y);
            set
            {
                _mouseScreenPos = value;
                value = new Vec2(value.x / Layer.HUD.camera.width * Resolution.size.x, value.y / Layer.HUD.camera.height * Resolution.size.y);
                Microsoft.Xna.Framework.Input.Mouse.SetPosition((int)value.x, (int)value.y);
            }
        }

        public static Vec2 mousePos => _mousePos;

        /// <summary>
        /// This mouse position variable is literally transformed to be NOT the screen position, and instead is the current layer in-game position
        /// "mousePos" is the ACTUAL screen-space position, if you need it. ~ Hyeve ~
        /// </summary>
        public static Vec2 positionScreen
        {
            get
            {
                if (Level.current == null || Level.current.camera == null)
                {
                    return Vec2.Zero;
                }
                return Level.current.camera.transformScreenVector(_mousePos);
            }
        }

        public static Vec2 positionConsole => Layer.Console.camera.transformScreenVector(_mousePos);

        public Mouse()
          : base()
        {
        }
    }
}
