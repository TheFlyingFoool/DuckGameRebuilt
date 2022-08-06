// Decompiled with JetBrains decompiler
// Type: XnaToFna.StubXDK.GamerServices.TitleServiceDirectory
// Assembly: XnaToFna, Version=18.5.1.29483, Culture=neutral, PublicKeyToken=null
// MVID: C1D3521D-C7E9-4C43-B430-D28CC69450A3
// Assembly location: C:\Users\daniel\Desktop\Release\XnaToFna.exe

using System;

namespace XnaToFna.StubXDK.GamerServices
{
  public class TitleServiceDirectory
  {
    private EventHandler<FindServicesCompletedArgs> _FindServicesCompleted;

    public event EventHandler<FindServicesCompletedArgs> FindServicesCompleted
    {
      add => this._FindServicesCompleted += value;
      remove => this._FindServicesCompleted -= value;
    }

    public bool IsBusy => false;

    public void FindServicesAsync()
    {
    }
  }
}
