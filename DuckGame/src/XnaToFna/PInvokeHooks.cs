using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SDL2;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using XnaToFna.ProxyForms;

namespace XnaToFna
{
    public static class PInvokeHooks
    {
        public static unsafe bool GetClipCursor(ref Rectangle rect)
        {
            fixed (Rectangle* rectanglePtr = &rect)
            {
                if ((long)rectanglePtr == 0L)
                    return true;
            }
            if (MouseEvents.Clip.HasValue)
            {
                rect = MouseEvents.Clip.Value;
            }
            else
            {
                DisplayMode currentDisplayMode = XnaToFnaHelper.Game.GraphicsDevice.Adapter.CurrentDisplayMode;
                rect = new Rectangle(0, 0, currentDisplayMode.Width, currentDisplayMode.Height);
            }
            return true;
        }


        public static unsafe bool ClipCursor(ref Rectangle rect)
        {
            fixed (Rectangle* rect_ = &rect)
            {
                if (rect_ == null)
                {
                    XnaToFnaHelper.Log("[CursorEvents] Cursor released from ClipCursor");
                    MouseEvents.Clip = null;
                    return true;
                }
            }
            XnaToFnaHelper.Log(string.Format("[CursorEvents] Game tries to ClipCursor inside {0}", rect));
            MouseEvents.Clip = new Rectangle?(rect);
            return true;
        }

        public static IntPtr GetForegroundWindow() => XnaToFnaHelper.Game.IsActive ? GameForm.Instance.Handle : IntPtr.Zero;

        public static bool SetForegroundWindow(IntPtr hWnd)
        {
            if (GameForm.Instance.Handle != hWnd)
                return false;
            SDL.SDL_RaiseWindow(XnaToFnaHelper.Game.Window.Handle);
            return true;
        }

        public static int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong)
        {
            if (nIndex != -4)
                return 0;
            Form form = Control.FromHandle(hWnd)?.Form;
            if (form == null)
                return 0;
            IntPtr windowHookPtr = form.WindowHookPtr;
            form.WindowHookPtr = (IntPtr)dwNewLong;
            form.WindowHook = Marshal.GetDelegateForFunctionPointer(form.WindowHookPtr, typeof(WndProc));
            XnaToFnaHelper.Log(string.Format("[PInvokeHooks] Window hook set on ProxyForms.Form #{0}", form.GlobalIndex));
            return (int)windowHookPtr;
        }

        public static IntPtr CallWindowProc(
          IntPtr lpPrevWndFunc,
          IntPtr hWnd,
          uint Msg,
          IntPtr wParam,
          IntPtr lParam)
        {
            if (lpPrevWndFunc == IntPtr.Zero)
                return IntPtr.Zero;
            return (IntPtr)Marshal.GetDelegateForFunctionPointer(lpPrevWndFunc, typeof(MulticastDelegate)).DynamicInvoke(hWnd, Msg, wParam, lParam);
        }

        public static IntPtr SetWindowsHookEx(
          HookType hookType,
          HookProc lpfn,
          IntPtr hMod,
          uint dwThreadId)
        {
            int num = PInvoke.AllHooks.Count + 1;
            List<Delegate> hook = PInvoke.Hooks[hookType];
            PInvoke.AllHooks.Add(Tuple.Create<HookType, Delegate, int>(hookType, lpfn, hook.Count));
            hook.Add(lpfn);
            XnaToFnaHelper.Log(string.Format("[PInvokeHooks] Added global hook #{0} of type {1}", num, hookType));
            return (IntPtr)num;
        }

        public static bool UnhookWindowsHookEx(IntPtr hhk)
        {
            int index = (int)hhk - 1;
            if (index < 0 || PInvoke.Hooks.Count <= index || PInvoke.AllHooks[index] == null)
                return true;
            Tuple<HookType, Delegate, int> allHook = PInvoke.AllHooks[index];
            PInvoke.AllHooks[index] = null;
            PInvoke.Hooks[allHook.Item1].RemoveAt(allHook.Item3);
            return true;
        }

        public static IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam) => PInvoke.ContinueHookChain(nCode, wParam, lParam);

        public static bool TranslateMessage(ref Message m) => true;

        public static unsafe uint GetWindowThreadProcessId(IntPtr hWnd, ref uint lpdwProcessId)
        {
            if (!(Control.FromHandle(hWnd) is Form form))
            {
                XnaToFnaHelper.Log(string.Format("[PInvokeHooks] Called GetWindowThreadProcessId for non-existing hWnd {0}", hWnd));
                form = GameForm.Instance;
            }
            fixed (uint* numPtr = &lpdwProcessId)
            {
                if ((long)numPtr != 0L)
                    lpdwProcessId = 0U;
            }
            return form == null ? 0U : (uint)form.ThreadId;
        }

        public static int GetCurrentThreadId() => (int)PInvokeHelper.CurrentThreadId;

        public static IntPtr LoadCursorFromFile(string str) => new Cursor(str).Handle;

        public static IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam)
        {
            if (!(Control.FromHandle(hWnd) is Form form))
            {
                XnaToFnaHelper.Log(string.Format("[PInvokeHooks] Called GetWindowThreadProcessId for non-existing hWnd {0}", hWnd));
                form = GameForm.Instance;
            }
            if (Msg != 16U)
                return IntPtr.Zero;
            form.Close();
            return IntPtr.Zero;
        }

        public static IntPtr ImmAssociateContext(IntPtr hWnd, IntPtr hIMC) => IntPtr.Zero;

        public static IntPtr ImmGetContext(IntPtr hWnd) => IntPtr.Zero;

        public static bool ImmReleaseContext(IntPtr hWnd, IntPtr hIMC) => true;

        public static short GetAsyncKeyState(int vKey) => Keyboard.GetState().IsKeyDown((Keys)vKey) ? (short)128 : (short)0;

        public static IntPtr LoadKeyboardLayout(string pwszKLID, uint Flags) => (IntPtr)1033;

        public static bool UnloadKeyboardLayout(IntPtr hkl) => true;

        public static unsafe bool GetKeyboardLayoutName(object pwszKLID)
        {
            switch (pwszKLID)
            {
                case StringBuilder _:
                    ((StringBuilder)pwszKLID).Append("00000409");
                    break;
                case IntPtr num:
                    char* pointer = (char*)num.ToPointer();
                    for (int index = 0; index < "00000409".Length; ++index)
                        pointer[index] = "00000409"[index];
                    break;
            }
            return true;
        }
    }
}
