// Decompiled with JetBrains decompiler
// Type: XnaToFna.StubXDK.StubXDKHelper
// Assembly: XnaToFna, Version=18.5.1.29483, Culture=neutral, PublicKeyToken=null
// MVID: C1D3521D-C7E9-4C43-B430-D28CC69450A3
// Assembly location: C:\Users\daniel\Desktop\Release\XnaToFna.exe

using System;
using System.Reflection;

namespace XnaToFna.StubXDK
{
  public static class StubXDKHelper
  {
    private static Assembly _GamerServicesAsm;

    public static Assembly GamerServicesAsm
    {
      get
      {
        if (StubXDKHelper._GamerServicesAsm != null)
          return StubXDKHelper._GamerServicesAsm;
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
          if (assembly.GetType("Microsoft.Xna.Framework.GamerServices.GamerPresence") != null)
            return StubXDKHelper._GamerServicesAsm = assembly;
        }
        return null;
      }
    }
  }
}
