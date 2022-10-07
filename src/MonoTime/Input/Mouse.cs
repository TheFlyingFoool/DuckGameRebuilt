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
                Mouse._outOfFocus = true;
            }
            else
            {
                Mouse._prevScrollValue = Mouse.scroll;
                Mouse._mouseStatePrev = Mouse._mouseState;
                Mouse._mouseState = Microsoft.Xna.Framework.Input.Mouse.GetState();
                Vec3 vec3 = new Vec3(_mouseState.X, _mouseState.Y, 0f);
                if (Graphics._screenViewport.HasValue)
                    Mouse._mouseScreenPos = new Vec2(vec3.x / Resolution.size.x * Layer.HUD.camera.width, vec3.y / Resolution.size.y * Layer.HUD.camera.height);
                Mouse._mouseScreenPos.x = (int)Mouse._mouseScreenPos.x;
                Mouse._mouseScreenPos.y = (int)Mouse._mouseScreenPos.y;
                Mouse._mousePos = new Vec2(_mouseState.X, _mouseState.Y);
                if (!Mouse._outOfFocus)
                    return;
                if (Mouse._mouseState.LeftButton == ButtonState.Released && Mouse._mouseState.MiddleButton == ButtonState.Released && Mouse._mouseState.RightButton == ButtonState.Released)
                    Mouse._outOfFocus = false;
                else
                    Mouse._mouseState = Mouse._mouseStatePrev = new MouseState(Mouse._mouseState.X, Mouse._mouseState.Y, Mouse._mouseState.ScrollWheelValue, ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released);
            }
        }

        public static InputState left
        {
            get
            {
                if (Mouse._mouseState.LeftButton == ButtonState.Pressed && Mouse._mouseStatePrev.LeftButton == ButtonState.Released)
                    return InputState.Pressed;
                if (Mouse._mouseState.LeftButton == ButtonState.Pressed && Mouse._mouseStatePrev.LeftButton == ButtonState.Pressed)
                    return InputState.Down;
                return Mouse._mouseState.LeftButton == ButtonState.Released && Mouse._mouseStatePrev.LeftButton == ButtonState.Pressed ? InputState.Released : InputState.None;
            }
        }

        public static InputState middle
        {
            get
            {
                if (Mouse._mouseState.MiddleButton == ButtonState.Pressed && Mouse._mouseStatePrev.MiddleButton == ButtonState.Released)
                    return InputState.Pressed;
                if (Mouse._mouseState.MiddleButton == ButtonState.Pressed && Mouse._mouseStatePrev.MiddleButton == ButtonState.Pressed)
                    return InputState.Down;
                return Mouse._mouseState.MiddleButton == ButtonState.Released && Mouse._mouseStatePrev.MiddleButton == ButtonState.Pressed ? InputState.Released : InputState.None;
            }
        }

        public static InputState right
        {
            get
            {
                if (Mouse._mouseState.RightButton == ButtonState.Pressed && Mouse._mouseStatePrev.RightButton == ButtonState.Released)
                    return InputState.Pressed;
                if (Mouse._mouseState.RightButton == ButtonState.Pressed && Mouse._mouseStatePrev.RightButton == ButtonState.Pressed)
                    return InputState.Down;
                return Mouse._mouseState.RightButton == ButtonState.Released && Mouse._mouseStatePrev.RightButton == ButtonState.Pressed ? InputState.Released : InputState.None;
            }
        }

        public static bool available => true;

        public static float scroll => Mouse._mouseStatePrev.ScrollWheelValue - Mouse._mouseState.ScrollWheelValue;
        /// <summary>
        /// scrolling but min/maxed so you dont get values that change based on how much you're scrolling
        /// </summary>
        public static int discreteScroll => scrollingUp ? -1 : scrollingDown ? 1 : 0;

        public static bool isScrolling => scroll != 0;
        public static bool scrollingDown => scroll > 0;
        public static bool scrollingUp => scroll < 0;

        public static bool prevScrollDown => _prevScrollValue > 0.0;

        public static bool prevScrollUp => _prevScrollValue < 0.0;

        public static float x => Mouse._mouseScreenPos.x;

        public static float y => Mouse._mouseScreenPos.y;

        public static float xScreen => Mouse.positionScreen.x;

        public static float yScreen => Mouse.positionScreen.y;

        public static float xConsole => Mouse.positionConsole.x;

        public static float yConsole => Mouse.positionConsole.y;

        public static Vec2 position
        {
            get => new Vec2(Mouse.x, Mouse.y);
            set
            {
                Mouse._mouseScreenPos = value;
                value = new Vec2(value.x / Layer.HUD.camera.width * Resolution.size.x, value.y / Layer.HUD.camera.height * Resolution.size.y);
                Microsoft.Xna.Framework.Input.Mouse.SetPosition((int)value.x, (int)value.y);
            }
        }

        public static Vec2 mousePos => Mouse._mousePos;

        /// <summary>
        /// This mouse position variable is literally transformed to be NOT the screen position, and instead is the current layer in-game position
        /// "mousePos" is the ACTUAL screen-space position, if you need it. ~ Hyeve ~
        /// </summary>
        public static Vec2 positionScreen => Level.current.camera.transformScreenVector(Mouse._mousePos);

        public static Vec2 positionConsole => Layer.Console.camera.transformScreenVector(Mouse._mousePos);

        public Mouse()
          : base()
        {
        }
    }
}
