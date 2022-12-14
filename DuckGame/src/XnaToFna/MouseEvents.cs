// Decompiled with JetBrains decompiler
// Type: XnaToFna.MouseEvents
// Assembly: XnaToFna, Version=18.5.1.29483, Culture=neutral, PublicKeyToken=null
// MVID: C1D3521D-C7E9-4C43-B430-D28CC69450A3
// Assembly location: C:\Users\daniel\Desktop\Release\XnaToFna.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using XnaToFna.ProxyForms;

namespace XnaToFna
{
    public static class MouseEvents
    {
        public static Rectangle? Clip;
        public static MouseState PreviousState;

        public static void Moved() => PInvoke.CallHooks(Messages.WM_MOUSEFIRST, IntPtr.Zero, IntPtr.Zero);

        public static void LMBDown() => PInvoke.CallHooks(Messages.WM_LBUTTONDOWN, IntPtr.Zero, IntPtr.Zero);

        public static void LMBUp() => PInvoke.CallHooks(Messages.WM_LBUTTONUP, IntPtr.Zero, IntPtr.Zero);

        public static void RMBDown() => PInvoke.CallHooks(Messages.WM_RBUTTONDOWN, IntPtr.Zero, IntPtr.Zero);

        public static void RMBUp() => PInvoke.CallHooks(Messages.WM_RBUTTONUP, IntPtr.Zero, IntPtr.Zero);

        public static void MMBDown() => PInvoke.CallHooks(Messages.WM_MBUTTONDOWN, IntPtr.Zero, IntPtr.Zero);

        public static void MMBUp() => PInvoke.CallHooks(Messages.WM_MBUTTONUP, IntPtr.Zero, IntPtr.Zero);

        public static void XMBDown(int mb) => PInvoke.CallHooks(Messages.WM_XBUTTONDOWN, (IntPtr)(mb << 16), IntPtr.Zero);

        public static void XMBUp(int mb) => PInvoke.CallHooks(Messages.WM_XBUTTONUP, (IntPtr)(mb << 16), IntPtr.Zero);

        public static void Wheel(int scroll) => PInvoke.CallHooks(Messages.WM_MOUSEWHEEL, (IntPtr)(scroll << 16), IntPtr.Zero);

        public static void Update()
        {
            MouseState state = Mouse.GetState();
            if (Clip.HasValue)
            {
                Rectangle clientBounds = XnaToFnaHelper.Game.Window.ClientBounds;
                int num1 = state.X + clientBounds.X;
                int num2 = state.Y + clientBounds.Y;
                Rectangle rectangle = Clip.Value;
                if (num1 < rectangle.Left)
                    num1 = rectangle.Left;
                else if (rectangle.Right <= num1)
                    num1 = rectangle.Right;
                if (num2 < rectangle.Top)
                    num2 = rectangle.Top;
                else if (rectangle.Bottom <= num2)
                    num2 = rectangle.Bottom;
                int x = num1 - clientBounds.X;
                int y = num2 - clientBounds.Y;
                if (x != state.X || y != state.Y)
                    Mouse.SetPosition(x, y);
                state = Mouse.GetState();
            }
            if (state.X != PreviousState.X || state.Y != PreviousState.Y)
                Moved();
            if (state.LeftButton == ButtonState.Pressed && PreviousState.LeftButton == ButtonState.Released)
                LMBDown();
            else if (state.LeftButton == ButtonState.Released && PreviousState.LeftButton == ButtonState.Pressed)
                LMBUp();
            if (state.RightButton == ButtonState.Pressed && PreviousState.RightButton == ButtonState.Released)
                RMBDown();
            else if (state.RightButton == ButtonState.Released && PreviousState.RightButton == ButtonState.Pressed)
                RMBUp();
            if (state.MiddleButton == ButtonState.Pressed && PreviousState.MiddleButton == ButtonState.Released)
                MMBDown();
            else if (state.MiddleButton == ButtonState.Released && PreviousState.MiddleButton == ButtonState.Pressed)
                MMBUp();
            if (state.XButton1 == ButtonState.Pressed && PreviousState.XButton1 == ButtonState.Released)
                XMBDown(1);
            else if (state.XButton1 == ButtonState.Released && PreviousState.XButton1 == ButtonState.Pressed)
                XMBUp(1);
            if (state.XButton2 == ButtonState.Pressed && PreviousState.XButton2 == ButtonState.Released)
                XMBDown(2);
            else if (state.XButton2 == ButtonState.Released && PreviousState.XButton2 == ButtonState.Pressed)
                XMBUp(2);
            if (state.ScrollWheelValue != PreviousState.ScrollWheelValue)
                Wheel(state.ScrollWheelValue - PreviousState.ScrollWheelValue);
            PreviousState = Mouse.GetState();
        }
    }
}
