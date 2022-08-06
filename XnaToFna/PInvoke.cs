// Decompiled with JetBrains decompiler
// Type: XnaToFna.PInvoke
// Assembly: XnaToFna, Version=18.5.1.29483, Culture=neutral, PublicKeyToken=null
// MVID: C1D3521D-C7E9-4C43-B430-D28CC69450A3
// Assembly location: C:\Users\daniel\Desktop\Release\XnaToFna.exe

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using XnaToFna.ProxyForms;

namespace XnaToFna
{
  public static class PInvoke
  {
    public static int MessageSize = Marshal.SizeOf(typeof (Message));
    public static Dictionary<HookType, List<Delegate>> Hooks = new Dictionary<HookType, List<Delegate>>();
    public static List<Tuple<HookType, Delegate, int>> AllHooks = new List<Tuple<HookType, Delegate, int>>();
    public static ThreadLocal<List<Delegate>> CurrentHookChain = new ThreadLocal<List<Delegate>>();
    public static ThreadLocal<int> CurrentHookIndex = new ThreadLocal<int>();

    static PInvoke()
    {
      foreach (HookType key in Enum.GetValues(typeof (HookType)))
        PInvoke.Hooks[key] = new List<Delegate>();
    }

    public static void CallHooks(
      Messages Msg,
      IntPtr wParam,
      IntPtr lParam,
      bool global = true,
      bool window = true,
      bool allWindows = false)
    {
      PInvoke.CallHooks(Msg, wParam, new Message()
      {
        HWnd = IntPtr.Zero,
        Msg = (int) Msg,
        WParam = wParam,
        LParam = lParam
      }, (global ? 1 : 0) != 0, (window ? 1 : 0) != 0, (allWindows ? 1 : 0) != 0);
    }

    public static void CallHooks(
      Messages Msg,
      IntPtr wParam,
      Message lParamMsg,
      bool global = true,
      bool window = true,
      bool allWindows = false)
    {
      IntPtr num = Marshal.AllocHGlobal(PInvoke.MessageSize);
      Marshal.StructureToPtr((object) lParamMsg, num, false);
      PInvoke.CallHooks(Msg, wParam, num, ref lParamMsg, global, window, allWindows);
      Marshal.FreeHGlobal(num);
    }

    public static void CallHooks(
      Messages Msg,
      IntPtr wParam,
      IntPtr lParam,
      ref Message lParamMsg,
      bool global = true,
      bool window = true,
      bool allWindows = false)
    {
      if (global)
        PInvoke.CallHookChain(HookType.WH_GETMESSAGE, (IntPtr) 1, lParam, ref lParamMsg);
      if (allWindows)
      {
        for (int index = 0; index < Control.AllControls.Count; ++index)
          lParamMsg.Result = PInvoke.CallWindowHook((IntPtr) (index + 1), Msg, wParam, lParam);
      }
      else
      {
        if (!window)
          return;
        lParamMsg.Result = PInvoke.CallWindowHook(Msg, wParam, lParam);
      }
    }

    public static IntPtr CallHookChain(
      HookType hookType,
      IntPtr wParam,
      IntPtr lParam,
      ref Message lParamMsg)
    {
      List<Delegate> hook = PInvoke.Hooks[hookType];
      if (hook.Count == 0)
        return IntPtr.Zero;
      PInvoke.CurrentHookChain.Value = hook;
      for (int index = 0; index < hook.Count; ++index)
      {
        Delegate @delegate = hook[index];
        if ((object) @delegate != null)
        {
          PInvoke.CurrentHookIndex.Value = index;
          object[] objArray = new object[3]
          {
            (object) 0,
            (object) wParam,
            (object) lParamMsg
          };
          object obj = @delegate.DynamicInvoke(objArray);
          lParamMsg = (Message) objArray[2];
          return obj == null ? IntPtr.Zero : (IntPtr) Convert.ToInt32(obj);
        }
      }
      return IntPtr.Zero;
    }

    public static IntPtr ContinueHookChain(int nCode, IntPtr wParam, IntPtr lParam)
    {
      List<Delegate> delegateList = PInvoke.CurrentHookChain.Value;
      for (int index = PInvoke.CurrentHookIndex.Value + 1; index < delegateList.Count; ++index)
      {
        Delegate @delegate = delegateList[index];
        if ((object) @delegate != null)
        {
          PInvoke.CurrentHookIndex.Value = index;
          return (IntPtr) @delegate.DynamicInvoke((object) (nCode < 0 ? nCode + 1 : 0), (object) wParam, (object) lParam);
        }
      }
      return IntPtr.Zero;
    }

    public static IntPtr CallWindowHook(Messages Msg, IntPtr wParam, IntPtr lParam)
    {
      GameForm instance = GameForm.Instance;
      return PInvoke.CallWindowHook(instance != null ? instance.Handle : IntPtr.Zero, (uint) Msg, wParam, lParam);
    }

    public static IntPtr CallWindowHook(
      IntPtr hWnd,
      Messages Msg,
      IntPtr wParam,
      IntPtr lParam)
    {
      return PInvoke.CallWindowHook(hWnd, (uint) Msg, wParam, lParam);
    }

    public static IntPtr CallWindowHook(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam)
    {
      if (!(Control.FromHandle(hWnd) is Form form) || form.WindowHookPtr == IntPtr.Zero)
        return IntPtr.Zero;
      return (IntPtr) form.WindowHook.DynamicInvoke((object) hWnd, (object) Msg, (object) wParam, (object) lParam);
    }
  }
}
