// Decompiled with JetBrains decompiler
// Type: Microsoft.Xna.Framework.Input.InputSystem
// Assembly: DuckGame, Version=1.1.8175.33388, Culture=neutral, PublicKeyToken=null
// MVID: C907F20B-C12B-4773-9B1E-25290117C0E4
// Assembly location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.exe
// XML documentation location: D:\Program Files (x86)\Steam\steamapps\common\Duck Game\DuckGame.xml

using DuckGame;
using System;
using System.Runtime.InteropServices;

namespace Microsoft.Xna.Framework.Input
{
    public static class InputSystem
    {
        private static bool initialized;
        private static IntPtr prevWndProc;
        private static InputSystem.WndProc hookProcDelegate;
        private static IntPtr hIMC;
        private const int GWL_WNDPROC = -4;
        private const int WM_KEYDOWN = 256;
        private const int WM_KEYUP = 257;
        private const int WM_CHAR = 258;
        private const int WM_IME_CHAR = 646;
        private const int WM_IME_SETCONTEXT = 641;
        private const int WM_INPUTLANGCHANGE = 81;
        private const int WM_GETDLGCODE = 135;
        private const int WM_IME_COMPOSITION = 271;
        private const int DLGC_WANTALLKEYS = 4;
        private const int WM_MOUSEMOVE = 512;
        private const int WM_LBUTTONDOWN = 513;
        private const int WM_LBUTTONUP = 514;
        private const int WM_LBUTTONDBLCLK = 515;
        private const int WM_RBUTTONDOWN = 516;
        private const int WM_RBUTTONUP = 517;
        private const int WM_RBUTTONDBLCLK = 518;
        private const int WM_MBUTTONDOWN = 519;
        private const int WM_MBUTTONUP = 520;
        private const int WM_MBUTTONDBLCLK = 521;
        private const int WM_ACTIVATEAPP = 28;
        private const int WM_MOUSEWHEEL = 522;
        private const int WM_XBUTTONDOWN = 523;
        private const int WM_XBUTTONUP = 524;
        private const int WM_XBUTTONDBLCLK = 525;
        private const int WM_MOUSEHOVER = 673;
        private static IntPtr hWND;

        /// <summary>Event raised when a character has been entered.</summary>
        public static event CharEnteredHandler IMECharEntered;

        /// <summary>Event raised when a character has been entered.</summary>
        public static event CharEnteredHandler CharEntered;

        /// <summary>
        /// Event raised when a key has been pressed down. May fire multiple times due to keyboard repeat.
        /// </summary>
        public static event KeyEventHandler KeyDown;

        /// <summary>Event raised when a key has been released.</summary>
        public static event KeyEventHandler KeyUp;

        [DllImport("Imm32.dll")]
        private static extern IntPtr ImmGetContext(IntPtr hWnd);

        [DllImport("Imm32.dll")]
        private static extern IntPtr ImmCreateContext();

        [DllImport("Imm32.dll")]
        private static extern IntPtr ImmAssociateContext(IntPtr hWnd, IntPtr hIMC);

        [DllImport("Imm32.dll")]
        private static extern bool ImmReleaseContext(IntPtr hWnd, IntPtr hIMC);

        [DllImport("Imm32.dll")]
        private static extern bool ImmSetOpenStatus(IntPtr hIMC, bool open);

        [DllImport("user32.dll")]
        private static extern IntPtr CallWindowProcW(
          IntPtr lpPrevWndFunc,
          IntPtr hWnd,
          uint Msg,
          IntPtr wParam,
          IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern int SetWindowLongW(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        private static extern bool IsWindowUnicode(IntPtr hWnd);

        public static bool ShiftDown
        {
            get
            {
                KeyboardState state = Keyboard.GetState();
                return state.IsKeyDown(Keys.LeftShift) || state.IsKeyDown(Keys.RightShift);
            }
        }

        public static bool CtrlDown
        {
            get
            {
                KeyboardState state = Keyboard.GetState();
                return state.IsKeyDown(Keys.LeftControl) || state.IsKeyDown(Keys.RightControl);
            }
        }

        public static bool AltDown
        {
            get
            {
                KeyboardState state = Keyboard.GetState();
                return state.IsKeyDown(Keys.LeftAlt) || state.IsKeyDown(Keys.RightAlt);
            }
        }

        /// <summary>Initialize the TextInput with the given GameWindow.</summary>
        /// <param name="window">The XNA window to which text input should be linked.</param>
        public static void Initialize(GameWindow window)
        {
            if (InputSystem.initialized)
                throw new InvalidOperationException("TextInput.Initialize can only be called once!");
            InputSystem.hookProcDelegate = new InputSystem.WndProc(InputSystem.HookProc);
            InputSystem.prevWndProc = (IntPtr)InputSystem.SetWindowLongW(window.Handle, -4, (int)Marshal.GetFunctionPointerForDelegate<InputSystem.WndProc>(InputSystem.hookProcDelegate));
            InputSystem.hWND = window.Handle;
        }

        public static void InitializeIme(GameWindow window)
        {
            InputSystem.hIMC = InputSystem.ImmCreateContext();
            InputSystem.ImmAssociateContext(InputSystem.hWND, InputSystem.hIMC);
        }

        public static void Terminate()
        {
            if (!InputSystem.initialized)
                return;
            InputSystem.ImmReleaseContext(InputSystem.hWND, InputSystem.hIMC);
        }

        public static void StartIME()
        {
            InputSystem.ImmAssociateContext(InputSystem.hWND, InputSystem.hIMC);
            InputSystem.ImmSetOpenStatus(InputSystem.hIMC, true);
            InputSystem.ImmReleaseContext(InputSystem.hWND, InputSystem.hIMC);
        }

        public static void EndIME()
        {
            InputSystem.ImmAssociateContext(InputSystem.hWND, InputSystem.hIMC);
            InputSystem.ImmSetOpenStatus(InputSystem.hIMC, false);
            InputSystem.ImmReleaseContext(InputSystem.hWND, InputSystem.hIMC);
        }

        private static IntPtr HookProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            MonoMain.ResetInfiniteLoopTimer();
            IntPtr num = InputSystem.CallWindowProcW(InputSystem.prevWndProc, hWnd, msg, wParam, lParam);
            if (msg == 28U)
                MonoMain.framesSinceFocusChange = !(wParam == IntPtr.Zero) ? 0 : 0;
            if (msg == 537U && wParam.ToInt32() == 7)
                DuckGame.Input.devicesChanged = true;
            if (Options.Data.imeSupport)
            {
                switch (msg)
                {
                    case 81:
                        InputSystem.ImmAssociateContext(hWnd, InputSystem.hIMC);
                        num = (IntPtr)1;
                        break;
                    case 135:
                        num = (IntPtr)(num.ToInt32() | 4);
                        break;
                    case 256:
                        if (InputSystem.KeyDown != null)
                        {
                            InputSystem.KeyDown((object)null, new KeyEventArgs((Keys)(int)wParam));
                            break;
                        }
                        break;
                    case 257:
                        if (InputSystem.KeyUp != null)
                        {
                            InputSystem.KeyUp((object)null, new KeyEventArgs((Keys)(int)wParam));
                            break;
                        }
                        break;
                    case 271:
                        DuckGame.Keyboard.isComposing = true;
                        break;
                    case 641:
                        if (wParam.ToInt32() == 1 && DuckGame.Input._imeAllowed)
                        {
                            InputSystem.ImmAssociateContext(hWnd, InputSystem.hIMC);
                            InputSystem.ImmSetOpenStatus(InputSystem.hIMC, true);
                            InputSystem.ImmReleaseContext(InputSystem.hWND, InputSystem.hIMC);
                            break;
                        }
                        break;
                    case 646:
                        if (InputSystem.IMECharEntered != null)
                        {
                            InputSystem.IMECharEntered((object)null, new CharacterEventArgs((char)(int)wParam, lParam.ToInt32()));
                            break;
                        }
                        break;
                }
            }
            return num;
        }

        private delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
    }
}
