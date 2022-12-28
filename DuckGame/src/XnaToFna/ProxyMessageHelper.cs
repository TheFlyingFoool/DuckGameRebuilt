// Decompiled with JetBrains decompiler
// Type: XnaToFna.ProxyForms.ProxyMessageHelper
// Assembly: XnaToFna, Version=18.5.1.29483, Culture=neutral, PublicKeyToken=null
// MVID: C1D3521D-C7E9-4C43-B430-D28CC69450A3
// Assembly location: C:\Users\daniel\Desktop\Release\XnaToFna.exe

using System;
using System.Security;
using System.Security.Permissions;
using System.Text;

namespace XnaToFna.ProxyForms
{
    public static class ProxyMessageHelper
    {
        public static readonly CodeAccessPermission UnmanagedCode = new SecurityPermission(SecurityPermissionFlag.UnmanagedCode);

        public static string MsgToString(int msg)
        {
            if (Enum.IsDefined(typeof(Messages), msg))
                return Enum.GetName(typeof(Messages), msg);
            return (msg & 8192) == 8192 ? "WM_REFLECT + " + (MsgToString(msg & -8193) ?? "???") : null;
        }

        public static StringBuilder Parenthesize(this StringBuilder builder, string input) => string.IsNullOrEmpty(input) ? builder : builder.Append(" (").Append(input).Append(")");

        public static string ToString(Message message) => ToString(message.HWnd, message.Msg, message.WParam, message.LParam, message.Result);

        public static string ToString(
          IntPtr hWnd,
          int msg,
          IntPtr wparam,
          IntPtr lparam,
          IntPtr result)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("msg=0x").Append(Convert.ToString(msg, 16)).Parenthesize(MsgToString(msg)).Append(" hwnd=0x").Append(Convert.ToString((long)hWnd, 16)).Append(" wparam=0x").Append(Convert.ToString((long)wparam, 16)).Append(" lparam=0x").Append(Convert.ToString((long)lparam, 16)).Parenthesize(msg == 528 ? MsgToString((int)wparam & ushort.MaxValue) : null).Append(" result=0x").Append(Convert.ToString((long)result, 16));
            return stringBuilder.ToString();
        }
    }
}
