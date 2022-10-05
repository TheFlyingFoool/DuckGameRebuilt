// Decompiled with JetBrains decompiler
// Type: XnaToFna.ProxyForms.Message
// Assembly: XnaToFna, Version=18.5.1.29483, Culture=neutral, PublicKeyToken=null
// MVID: C1D3521D-C7E9-4C43-B430-D28CC69450A3
// Assembly location: C:\Users\daniel\Desktop\Release\XnaToFna.exe

using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

namespace XnaToFna.ProxyForms
{
  [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
  public struct Message
  {
    public IntPtr HWnd { get; set; }

    public int Msg { get; set; }

    public IntPtr WParam { get; set; }

    public IntPtr LParam { get; set; }

    public IntPtr Result { get; set; }

    public object GetLParam(Type cls) => Marshal.PtrToStructure(this.LParam, cls);

    public static Message Create(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam) => new Message()
    {
      HWnd = hWnd,
      Msg = msg,
      WParam = wparam,
      LParam = lparam,
      Result = IntPtr.Zero
    };

    public override bool Equals(object o) => o is Message message && this.HWnd == message.HWnd && this.Msg == message.Msg && this.WParam == message.WParam && this.LParam == message.LParam && this.Result == message.Result;

    public static bool operator !=(Message a, Message b) => !a.Equals(b);

    public static bool operator ==(Message a, Message b) => a.Equals(b);

    public override int GetHashCode() => (int) this.HWnd << 4 | this.Msg;

    public override string ToString()
    {
      bool flag = false;
      try
      {
        ProxyMessageHelper.UnmanagedCode.Demand();
        flag = true;
      }
      catch (SecurityException ex)
      {
      }
      return !flag ? base.ToString() : ProxyMessageHelper.ToString(this);
    }
  }
}
