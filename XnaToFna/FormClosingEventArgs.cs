// Decompiled with JetBrains decompiler
// Type: XnaToFna.ProxyForms.FormClosingEventArgs
// Assembly: XnaToFna, Version=18.5.1.29483, Culture=neutral, PublicKeyToken=null
// MVID: C1D3521D-C7E9-4C43-B430-D28CC69450A3
// Assembly location: C:\Users\daniel\Desktop\Release\XnaToFna.exe

using System.ComponentModel;

namespace XnaToFna.ProxyForms
{
  public class FormClosingEventArgs : CancelEventArgs
  {
    public CloseReason CloseReason { get; private set; }

    public FormClosingEventArgs(CloseReason closeReason, bool cancel)
      : base(cancel)
    {
      this.CloseReason = closeReason;
    }
  }
}
