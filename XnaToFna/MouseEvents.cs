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

    public static void XMBDown(int mb) => PInvoke.CallHooks(Messages.WM_XBUTTONDOWN, (IntPtr) (mb << 16), IntPtr.Zero);

    public static void XMBUp(int mb) => PInvoke.CallHooks(Messages.WM_XBUTTONUP, (IntPtr) (mb << 16), IntPtr.Zero);

    public static void Wheel(int scroll) => PInvoke.CallHooks(Messages.WM_MOUSEWHEEL, (IntPtr) (scroll << 16), IntPtr.Zero);

    public static void Update()
    {
      MouseState state = Mouse.GetState();
      if (MouseEvents.Clip.HasValue)
      {
        Rectangle clientBounds = XnaToFnaHelper.Game.Window.ClientBounds;
        int num1 = state.X + clientBounds.X;
        int num2 = state.Y + clientBounds.Y;
        Rectangle rectangle = MouseEvents.Clip.Value;
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
      if (state.X != MouseEvents.PreviousState.X || state.Y != MouseEvents.PreviousState.Y)
        MouseEvents.Moved();
      if (state.LeftButton == ButtonState.Pressed && MouseEvents.PreviousState.LeftButton == ButtonState.Released)
        MouseEvents.LMBDown();
      else if (state.LeftButton == ButtonState.Released && MouseEvents.PreviousState.LeftButton == ButtonState.Pressed)
        MouseEvents.LMBUp();
      if (state.RightButton == ButtonState.Pressed && MouseEvents.PreviousState.RightButton == ButtonState.Released)
        MouseEvents.RMBDown();
      else if (state.RightButton == ButtonState.Released && MouseEvents.PreviousState.RightButton == ButtonState.Pressed)
        MouseEvents.RMBUp();
      if (state.MiddleButton == ButtonState.Pressed && MouseEvents.PreviousState.MiddleButton == ButtonState.Released)
        MouseEvents.MMBDown();
      else if (state.MiddleButton == ButtonState.Released && MouseEvents.PreviousState.MiddleButton == ButtonState.Pressed)
        MouseEvents.MMBUp();
      if (state.XButton1 == ButtonState.Pressed && MouseEvents.PreviousState.XButton1 == ButtonState.Released)
        MouseEvents.XMBDown(1);
      else if (state.XButton1 == ButtonState.Released && MouseEvents.PreviousState.XButton1 == ButtonState.Pressed)
        MouseEvents.XMBUp(1);
      if (state.XButton2 == ButtonState.Pressed && MouseEvents.PreviousState.XButton2 == ButtonState.Released)
        MouseEvents.XMBDown(2);
      else if (state.XButton2 == ButtonState.Released && MouseEvents.PreviousState.XButton2 == ButtonState.Pressed)
        MouseEvents.XMBUp(2);
      if (state.ScrollWheelValue != MouseEvents.PreviousState.ScrollWheelValue)
        MouseEvents.Wheel(state.ScrollWheelValue - MouseEvents.PreviousState.ScrollWheelValue);
      MouseEvents.PreviousState = Mouse.GetState();
    }
  }
}
