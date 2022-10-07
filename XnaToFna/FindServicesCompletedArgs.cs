// Decompiled with JetBrains decompiler
// Type: XnaToFna.StubXDK.GamerServices.FindServicesCompletedArgs
// Assembly: XnaToFna, Version=18.5.1.29483, Culture=neutral, PublicKeyToken=null
// MVID: C1D3521D-C7E9-4C43-B430-D28CC69450A3
// Assembly location: C:\Users\daniel\Desktop\Release\XnaToFna.exe

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace XnaToFna.StubXDK.GamerServices
{
    public class FindServicesCompletedArgs : EventArgs
    {
        public ReadOnlyCollection<TitleServiceDescription> Services => new ReadOnlyCollection<TitleServiceDescription>(new List<TitleServiceDescription>());
    }
}
