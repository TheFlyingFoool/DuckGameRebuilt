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

        public static void KeyDown(Keys key) => PInvoke.CallHooks(Messages.WM_KEYDOWN, (IntPtr)(int)key, IntPtr.Zero);

        public static void KeyUp(Keys key) => PInvoke.CallHooks(Messages.WM_KEYUP, (IntPtr)(int)key, IntPtr.Zero);

        public static void CharEntered(char c) => PInvoke.CallHooks(Messages.WM_CHAR, (IntPtr)(int)c, IntPtr.Zero);

        public static void SetContext(bool wParam) => PInvoke.CallHooks(Messages.WM_IME_SETCONTEXT, (IntPtr)(wParam ? 1 : 0), IntPtr.Zero);

        public static void Update()
        {
            Keys[] pressedKeys = Keyboard.GetState().GetPressedKeys();
            Down.Clear();
            for (int index = 0; index < pressedKeys.Length; ++index)
            {
                Keys key = pressedKeys[index];
                if (!LastDown.Contains(key))
                    KeyDown(key);
                Down.Add(key);
            }
            foreach (Keys key in LastDown)
            {
                if (!Down.Contains(key))
                    KeyUp(key);
            }
            LastDown.Clear();
            LastDown.UnionWith(Down);
        }
    }
}
