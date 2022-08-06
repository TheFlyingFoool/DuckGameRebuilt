// Decompiled with JetBrains decompiler
// Type: XnaToFna.KeyboardEvents
// Assembly: XnaToFna, Version=18.5.1.29483, Culture=neutral, PublicKeyToken=null
// MVID: C1D3521D-C7E9-4C43-B430-D28CC69450A3
// Assembly location: C:\Users\daniel\Desktop\Release\XnaToFna.exe

using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using XnaToFna.ProxyForms;

namespace XnaToFna
{
  public static class KeyboardEvents
  {
    public static HashSet<Keys> LastDown = new HashSet<Keys>();
    public static HashSet<Keys> Down = new HashSet<Keys>();

    public static void KeyDown(Keys key) => PInvoke.CallHooks(Messages.WM_KEYDOWN, (IntPtr) (int) key, IntPtr.Zero);

    public static void KeyUp(Keys key) => PInvoke.CallHooks(Messages.WM_KEYUP, (IntPtr) (int) key, IntPtr.Zero);

    public static void CharEntered(char c) => PInvoke.CallHooks(Messages.WM_CHAR, (IntPtr) (int) c, IntPtr.Zero);

    public static void SetContext(bool wParam) => PInvoke.CallHooks(Messages.WM_IME_SETCONTEXT, (IntPtr) (wParam ? 1 : 0), IntPtr.Zero);

    public static void Update()
    {
      Keys[] pressedKeys = Keyboard.GetState().GetPressedKeys();
      KeyboardEvents.Down.Clear();
      for (int index = 0; index < pressedKeys.Length; ++index)
      {
        Keys key = pressedKeys[index];
        if (!KeyboardEvents.LastDown.Contains(key))
          KeyboardEvents.KeyDown(key);
        KeyboardEvents.Down.Add(key);
      }
      foreach (Keys key in KeyboardEvents.LastDown)
      {
        if (!KeyboardEvents.Down.Contains(key))
          KeyboardEvents.KeyUp(key);
      }
      KeyboardEvents.LastDown.Clear();
      KeyboardEvents.LastDown.UnionWith((IEnumerable<Keys>) KeyboardEvents.Down);
    }
  }
}
